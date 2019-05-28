using RimWorld;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public class CompProperties_InfiniteEnergySource : CompProperties
    {
        public CompProperties_InfiniteEnergySource()
        {
            compClass = typeof(Comp_InfiniteEnergySource);
        }
    }

    public class Comp_InfiniteEnergySource : ThingComp, IEnergySource
    {
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
            return IsActive() ? amount : 0f;
        }

        public void Drain(float amount)
        {
        }
    }
}