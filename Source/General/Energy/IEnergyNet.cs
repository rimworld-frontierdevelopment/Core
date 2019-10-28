using FrontierDevelopments.General.Energy;

namespace FrontierDevelopments.General
{
    public interface IEnergyNet : IEnergyNode
    {
        void Connect(IEnergyNode node);
        void Disconnect(IEnergyNode node);
    }
}