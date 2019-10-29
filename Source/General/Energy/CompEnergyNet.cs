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
        private readonly EnergyNet _energyNet = new EnergyNet();

        public IEnumerable<IEnergyNode> Nodes => _energyNet.Nodes;

        public void Connect(IEnergyNode node)
        {
            _energyNet.Connect(node);
        }

        public void Disconnect(IEnergyNode node)
        {
            _energyNet.Disconnect(node);
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
    }
}