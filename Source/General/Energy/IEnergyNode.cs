namespace FrontierDevelopments.General.Energy
{
    public interface IEnergyNode
    {
        /// <summary>
        /// Amount of energy the network can provide.
        /// </summary>
        float AmountAvailable { get; }

        /// <summary>
        /// Amount of energy the network can provide per tick.
        /// </summary>
        float RateAvailable { get; }

        /// <summary>
        /// Provide the net work with energy.
        /// </summary>
        /// <param name="amount">Amount to provide</param>
        /// <returns>Energy stored</returns>
        float Provide(float amount);

        /// <summary>
        /// Consume energy from the network. Will consume energy up to amount even if the
        /// amount can't be filled.
        /// </summary>
        /// <param name="amount">Amount requested</param>
        /// <returns>Energy consumed</returns>
        float Consume(float amount);
    }
}