using FrontierDevelopments.General.Energy;
using System.Collections.Generic;
using Verse;

namespace FrontierDevelopments.General
{
    public interface IEnergyNet : IEnergyProvider, IEnergyConsumer, ILoadReferenceable
    {
        IEnumerable<IEnergyNode> Nodes { get; }

        void Connect(IEnergyNode node);

        void Disconnect(IEnergyNode node);

        /// <summary>
        /// Fired when a connected node needs to update.
        /// </summary>
        void Changed();
    }
}