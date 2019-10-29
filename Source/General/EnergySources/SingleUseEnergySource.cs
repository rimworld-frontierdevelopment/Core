using FrontierDevelopments.General.Energy;
using Verse;
using Verse.Noise;

namespace FrontierDevelopments.General.EnergySources
{
    public class CompProperties_SingleUseEnergySource : CompProperties
    {
        public float charge;
        public float rate = float.PositiveInfinity;

        public CompProperties_SingleUseEnergySource()
        {
            compClass = typeof(Comp_SingleUseEnergySource);
        }
    }

    public class Comp_SingleUseEnergySource : ThingComp, IEnergyNode
    {
        protected float _charge = -1f;

        private CompProperties_SingleUseEnergySource Props => (CompProperties_SingleUseEnergySource) props;

        public virtual float MinimumCharge => 0f;

        public float AmountAvailable => _charge;

        public float RateAvailable => Props.rate;

        public float TotalAvailable => Props.charge;

        public float MaxRate => Props.rate;

        public float Provide(float amount)
        {
            return 0f;
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (_charge < 0) _charge = Props.charge;
        }

        public override string CompInspectStringExtra()
        {
            return "Charge: " + _charge;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _charge, "charge", -1);
        }

        public float Consume(float amount)
        {
            if (amount >= _charge)
            {
                _charge = 0f;
                OnEmpty();
                return _charge;
            }
            _charge -= amount;
            return amount;
        }

        protected virtual void OnEmpty()
        {
        }
    }
}
