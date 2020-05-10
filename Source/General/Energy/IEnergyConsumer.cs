namespace FrontierDevelopments.General.Energy
{
    public interface IEnergyConsumer : IEnergyNode
    {
        /// <summary>
        /// The rate which this node consumes energy.
        /// </summary>
        float Rate { get; }

        /// <summary>
        /// Signal fired from a power net that indicates whether or not this device is powered.
        /// </summary>
        /// <param name="isPowered"></param>
        void HasPower(bool isPowered);
    }
}