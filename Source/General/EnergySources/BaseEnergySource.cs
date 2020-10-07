using System;
using System.Linq;
using FrontierDevelopments.General.Energy;
using Verse;

namespace FrontierDevelopments.General.EnergySources
{
    public abstract class BaseEnergySource : ThingComp, IEnergyProvider
    {
        protected float DrawThisTick;
        private IEnergyNet _parent;

        public IEnergyNet Parent => _parent;

        public abstract float AmountAvailable { get; }
        public abstract float TotalAvailable { get; }
        public virtual float RateAvailable => Math.Min(MaxRate - DrawThisTick, AmountAvailable);
        public abstract float MaxRate { get; }

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
            DrawThisTick += amount;
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
            DrawThisTick = 0;
        }

        public override void PostExposeData()
        {
            Scribe_References.Look(ref _parent, SaveKey + "NetParent");
            Scribe_Values.Look(ref DrawThisTick, SaveKey + "DrawThisTick");
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ConnectTo(_parent);
            }
        }
    }
}