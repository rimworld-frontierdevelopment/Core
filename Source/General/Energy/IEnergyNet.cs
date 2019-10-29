using System.Collections.Generic;
using FrontierDevelopments.General.Energy;

namespace FrontierDevelopments.General
{
    public interface IEnergyNet : IEnergyNode
    {
        IEnumerable<IEnergyNode> Nodes { get; }

        void Connect(IEnergyNode node);
        void Disconnect(IEnergyNode node);
    }
}