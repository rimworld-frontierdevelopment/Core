using FrontierDevelopments.General.Energy;
using System;
using System.Linq;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public abstract class BaseEnergySource : ThingComp, IEnergyProvider
    {
        private float _drawThisTick;
        private IEnergyNet _parent;

        public IEnergyNet Parent => _parent;

        public virtual float AmountAvailable => throw new Exception();
        public virtual float TotalAvailable => throw new Exception();
        public virtual float RateAvailable => Math.Min(MaxRate - _drawThisTick, AmountAvailable);
        public virtual float MaxRate => throw new Exception();

        protected abstract string SaveKey { get; }

        public void ConnectTo(IEnergyNet net)
        {
            _parent?.Disconnect(this);
            _parent = net;
            _parent?.Connect(this);
        }

        public void Disconnect()
        {
            _parent?.Disconnect(this);
            _parent = null;
        }

        public virtual float Provide(float amount)
        {
            return 0f;
        }

        public virtual float Consume(float amount)
        {
            return Request(amount);
        }

        public virtual float Request(float amount)
        {
            if (amount > RateAvailable) amount = RateAvailable;
            _drawThisTick += amount;
            return amount;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            ConnectTo(parent.AllComps.OfType<Object>().Concat(parent).OfType<IEnergyNet>().First());
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            _parent?.Disconnect(this);
        }

        public void Update()
        {
            _drawThisTick = 0;
        }

        public override void PostExposeData()
        {
            Scribe_References.Look(ref _parent, SaveKey + "NetParent");
            Scribe_Values.Look(ref _drawThisTick, SaveKey + "DrawThisTick");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ConnectTo(_parent);
            }
        }
    }
}