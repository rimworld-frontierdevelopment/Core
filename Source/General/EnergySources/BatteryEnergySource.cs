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
    
    public class BatteryEnergySource: ThingComp, IEnergySource
    {
        private float _baseConsumption;

        private CompPowerBattery Battery => parent.GetComp<CompPowerBattery>();

        public bool IsActive()
        {
            return EnergyAvailable > 0;
        }

        public bool WantActive => true;

        public float BaseConsumption
        {
            get => _baseConsumption;
            set => _baseConsumption = value;
        }

        public float EnergyAvailable => Battery.StoredEnergy;
        public float Draw(float amount)
        {
            if (amount > EnergyAvailable)
            {
                Battery.DrawPower(EnergyAvailable);
                return EnergyAvailable;
            }
            else
            {
                Battery.DrawPower(amount);
                return amount;
            }
        }

        public void Drain(float amount)
        {
            Draw(amount);
        }

        public override void CompTick()
        {
            if (_baseConsumption > 0)
            {
                Battery.DrawPower(_baseConsumption);
            }
        }
    }
}