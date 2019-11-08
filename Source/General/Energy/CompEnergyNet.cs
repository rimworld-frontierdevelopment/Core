using System.Collections.Generic;
using Verse;

namespace FrontierDevelopments.General.Energy
{
    public class CompPropertiesEnergyNet : CompProperties
    {
        public CompPropertiesEnergyNet()
        {
            compClass = typeof(CompEnergyNet);
        }
    }

    public class CompEnergyNet : ThingComp, IEnergyNet
    {
        private EnergyNet _energyNet = new EnergyNet();

        public IEnergyNet Parent => _energyNet.Parent;
        
        public IEnumerable<IEnergyNode> Nodes => _energyNet.Nodes;

        public void Connect(IEnergyNode node)
        {
            _energyNet.Connect(node);
        }

        public void Disconnect(IEnergyNode node)
        {
            _energyNet.Disconnect(node);
        }

        public void ConnectTo(IEnergyNet net)
        {
            _energyNet.ConnectTo(net);
        }

        public void Disconnect()
        {
            _energyNet.Disconnect();
        }

        public float AmountAvailable => _energyNet.AmountAvailable;
        public float RateAvailable => _energyNet.RateAvailable;
        public float TotalAvailable => _energyNet.TotalAvailable;
        public float MaxRate => _energyNet.MaxRate;

        public float Provide(float amount)
        {
            return _energyNet.Provide(amount);
        }

        public float Consume(float amount)
        {
            return _energyNet.Consume(amount);
        }

        public float Request(float amount)
        {
            return _energyNet.Request(amount);
        }

        public void Update()
        {
            switch (parent)
            {
                case Pawn pawn:
                    if (Find.WorldPawns.Contains(pawn)) return;
                    break;
            }
            _energyNet.Update();
        }

        public void Changed()
        {
            _energyNet.Changed();
        }

        public override void CompTick()
        {
            Update();
        }

        public float Rate => _energyNet.Rate;
        public void HasPower(bool isPowered)
        {
            _energyNet.HasPower(isPowered);
        }

        public string GetUniqueLoadID()
        {
            return _energyNet.GetUniqueLoadID();
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref _energyNet, "energyNet");
        }
    }
}