using RimWorld;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public class CompProperties_FixedEnergySource : CompProperties
    {
        public int amount;

        public CompProperties_FixedEnergySource()
        {
            compClass = typeof(Comp_FixedEnergySource);
        }
    }
    
    public class Comp_FixedEnergySource : ThingComp, IEnergySource
    {
        private CompProperties_FixedEnergySource Props => (CompProperties_FixedEnergySource) props;

        private CompFlickable _flickable;
        
        public float BaseConsumption { get => 0f; set {} }

        public bool WantActive => _flickable?.SwitchIsOn ?? true;

        public float EnergyAvailable => float.PositiveInfinity;

        public bool IsActive()
        {
            return WantActive;
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            _flickable = parent.GetComp<CompFlickable>();
        }

        public float Draw(float amount)
        {
            if (IsActive() && amount <= Props.amount)
            {
                return amount;
            }

            return Props.amount;
        }

        public void Drain(float amount)
        {
        }
    }
}
