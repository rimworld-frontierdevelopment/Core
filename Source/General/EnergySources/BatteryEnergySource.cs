using RimWorld;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public class BatteryEnergySourceProperties : CompProperties
    {
        public float rate = float.PositiveInfinity;

        public BatteryEnergySourceProperties()
        {
            compClass = typeof(BatteryEnergySource);
        }
    }

    public class BatteryEnergySource : BaseEnergySource
    {
        protected override string SaveKey => "BatterySource";

        private CompPowerBattery Battery => parent.GetComp<CompPowerBattery>();

        public override float Provide(float amount)
        {
            var toStore = amount;

            if (amount > Battery.AmountCanAccept)
                toStore = Battery.AmountCanAccept;

            Battery.AddEnergy(toStore);

            return amount - toStore;
        }

        public override float Consume(float amount)
        {
            amount = base.Consume(amount);
            if (amount > AmountAvailable)
            {
                Battery.DrawPower(AmountAvailable);
                return AmountAvailable;
            }
            else
            {
                Battery.DrawPower(amount);
                return amount;
            }
        }

        public override float AmountAvailable => Battery.StoredEnergy;

        public override float TotalAvailable => Battery.Props.storedEnergyMax;

        public override float MaxRate => ((BatteryEnergySourceProperties)props).rate;
    }
}