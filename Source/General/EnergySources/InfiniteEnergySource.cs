using FrontierDevelopments.General.Energy;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public class CompProperties_InfiniteEnergySource : CompProperties
    {
        public float rate = float.PositiveInfinity;

        public CompProperties_InfiniteEnergySource()
        {
            compClass = typeof(Comp_InfiniteEnergySource);
        }
    }

    public class Comp_InfiniteEnergySource : ThingComp, IEnergyNode
    {
        private CompProperties_InfiniteEnergySource Props => (CompProperties_InfiniteEnergySource) props;

        public float AmountAvailable => float.PositiveInfinity;

        public float RateAvailable => Props.rate;

        public float TotalAvailable => float.PositiveInfinity;

        public float MaxRate => Props.rate;

        public float Provide(float amount)
        {
            return 0f;
        }

        public float Consume(float amount)
        {
            return amount > RateAvailable ? RateAvailable : amount;
        }
    }
}