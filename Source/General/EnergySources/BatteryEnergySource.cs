using FrontierDevelopments.General.Energy;
using RimWorld;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public class BatteryEnergySourceProperties : CompProperties
    {
        public BatteryEnergySourceProperties()
        {
            compClass = typeof(BatteryEnergySource);
        }
    }

    public class BatteryEnergySource: ThingComp, IEnergyNode
    {
        private CompPowerBattery Battery => parent.GetComp<CompPowerBattery>();

        public float Provide(float amount)
        {
            var toStore = amount;

            if (amount > Battery.AmountCanAccept)
                toStore = Battery.AmountCanAccept;

            Battery.AddEnergy(toStore);

            return amount - toStore;
        }

        public float Consume(float amount)
        {
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

        public float AmountAvailable => Battery.StoredEnergy;

        public float RateAvailable => AmountAvailable;
    }
}