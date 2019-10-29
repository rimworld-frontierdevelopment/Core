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

    public class Comp_InfiniteEnergySource : BaseEnergySource
    {
        private CompProperties_InfiniteEnergySource Props => (CompProperties_InfiniteEnergySource) props;

        public override float AmountAvailable => float.PositiveInfinity;

        public override float TotalAvailable => float.PositiveInfinity;

        public override float MaxRate => Props.rate;

        public override float Provide(float amount)
        {
            return 0f;
        }

        public override float Consume(float amount)
        {
            amount = base.Consume(amount);
            return amount > RateAvailable ? RateAvailable : amount;
        }
    }
}