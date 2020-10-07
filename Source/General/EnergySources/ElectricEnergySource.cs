using System;
using System.Linq;
using RimWorld;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public class CompProperties_ElectricEnergySource : CompProperties
    {
        public float rate = float.PositiveInfinity;
        public float minimumOnlinePower;
        
        public CompProperties_ElectricEnergySource()
        {
            compClass = typeof(Comp_ElectricEnergySource);
        }
    }

    public class Comp_ElectricEnergySource : BaseEnergySource
    {
        protected override string SaveKey => "ElectricSource";
        
        private CompPowerTrader _powerTrader;

        private CompProperties_ElectricEnergySource Props => (CompProperties_ElectricEnergySource) props;
        
        public override float AmountAvailable => GainEnergyAvailable + StoredEnergyAvailable;

        public override float RateAvailable
        {
            get
            {
                if (!IsActive() || !WantOnline()) return 0f;
                return base.RateAvailable;
            }
        }

        public override float MaxRate => Math.Min(AmountAvailable, Props.rate);

        public override float TotalAvailable => GainEnergyAvailable + StoredEnergyTotal;

        private bool IsActive()
        {
            return CanBeOnline() && WantOnline() && base.RateAvailable >= Props.minimumOnlinePower;
        }

        private bool WantOnline()
        {
            return FlickUtility.WantsToBeOn(parent);
        }

        private bool CanBeOnline()
        {
            return _powerTrader != null;
        }

        private float GainEnergyAvailable => GainEnergyRate / GenDate.TicksPerDay;

        private float StoredEnergyAvailable => _powerTrader?.PowerNet?.CurrentStoredEnergy() ?? 0f;

        private float StoredEnergyTotal =>
            _powerTrader?.PowerNet?.batteryComps.Aggregate(0f, (sum, battery) => sum + battery.Props.storedEnergyMax) ?? 0f;

        private float GainEnergyRate => _powerTrader?.PowerNet?.CurrentEnergyGainRate() ?? 0f;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _powerTrader = parent.GetComp<CompPowerTrader>();
        }

        // Do the actual draw
        public override void CompTick()
        {
            // use base.RateAvailable to get around current tick draw contention problems
            // if we have an amount to draw this tick we need to draw it to be sure we can draw out the last power
            if (DrawThisTick > 0)
            {
                _powerTrader.PowerOutput = - (MaxRate - base.RateAvailable) * GenDate.TicksPerDay;
            }
            else
            {
                _powerTrader.PowerOutput = 0;
            }

            base.CompTick();
        }

        public override float Provide(float amount)
        {
            return _powerTrader?.PowerNet?.batteryComps.Aggregate(0f, (stored, battery) =>
            {
                var toStore = amount - stored;
                if (toStore > battery.AmountCanAccept)
                    toStore = battery.AmountCanAccept;
                battery.AddEnergy(toStore);
                return  stored + toStore;
            }) ?? 0f;
        }

        public override float Consume(float amount)
        {
            // figure out how much can be covered by network power
            // this will have to wait until the next tick to resolve
            // we can be at most that wrong if we attempt to overdraw for next tick
            // TODO create a manager for PowerNets that can detect draw contention
            var possibleShortFall = amount - base.Consume(amount);
            if (possibleShortFall < 0)
            {
                // not enough energy is stored
                return -possibleShortFall;
            }

            // good to go!
            return amount;
        }
    }
}
