namespace FrontierDevelopments.General.Energy
{
    public interface IEnergyNode
    {
        IEnergyNet Parent { get; }

        void ConnectTo(IEnergyNet net);

        void Disconnect();
    }
}