using FrontierDevelopments.General.Energy;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public abstract class BaseEnergySource : ThingComp, IEnergyNode
    {
        private float _drawThisTick;

        public virtual float AmountAvailable { get; }
        public virtual float TotalAvailable { get; }
        public virtual float RateAvailable => MaxRate - _drawThisTick;
        public virtual float MaxRate { get; }

        public virtual float Provide(float amount)
        {
            return 0f;
        }

        public virtual float Consume(float amount)
        {
            if (amount > RateAvailable) amount = RateAvailable;
            _drawThisTick += amount;
            return amount;
        }

        public override void CompTick()
        {
            _drawThisTick = 0;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _drawThisTick, "drawThisTick");
        }
    }
}