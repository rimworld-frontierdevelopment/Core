using System;
using System.Linq;
using FrontierDevelopments.General.Energy;
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

    public class Comp_ElectricEnergySource : BaseEnergySource, IEnergyNode
    {
        private CompPowerTrader _powerTrader;

        private CompProperties_ElectricEnergySource Props => (CompProperties_ElectricEnergySource) props;
        
        public override float AmountAvailable => GainEnergyAvailable + StoredEnergyAvailable;

        public override float RateAvailable
        {
            get
            {
                if (!IsActive()) return 0f;
                return base.RateAvailable;
            }
        }

        public override float MaxRate => Math.Min(AmountAvailable * GenDate.TicksPerDay, Props.rate);

        public override float TotalAvailable => GainEnergyAvailable + StoredEnergyTotal;

        private bool IsActive()
        {
            return Online() && base.RateAvailable >= Props.minimumOnlinePower;
        }

        private bool Online()
        {
            return _powerTrader != null && _powerTrader.PowerOn;
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
            if (Online())
            {
                _powerTrader.PowerOutput = - (MaxRate - RateAvailable) * GenDate.TicksPerDay;
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
            if (!IsActive()) return 0f;
            // figure out how much can be covered by network power
            // this will have to wait until the next tick to resolve
            // we can be at most that wrong if we attempt to overdraw for next tick
            // TODO create a manager for PowerNets that can detect draw contention
            var la = base.Consume(amount);

            var possibleShortFall = amount - la;
            
            
            
            if (possibleShortFall < 0)
            {
                // not enough energy is stored
                return -possibleShortFall;
            }
            else
            {
                // good to go!
                return amount;
            }
        }
    }
}
