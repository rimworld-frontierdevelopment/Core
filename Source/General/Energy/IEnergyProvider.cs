namespace FrontierDevelopments.General.Energy
{
    public interface IEnergyProvider : IEnergyNode
    {
        /// <summary>
        /// Amount of energy the network can provide.
        /// </summary>
        float AmountAvailable { get; }
        
        /// <summary>
        /// Total amount of energy the node can provide.
        /// </summary>
        float TotalAvailable { get; }
        
        /// <summary>
        /// Amount of energy the network can provide per tick.
        /// </summary>
        float RateAvailable { get; }
        
        /// <summary>
        /// The maximum rate the node can provide.
        /// </summary>
        float MaxRate { get; }
        
        /// <summary>
        /// Consume energy from the network. Will consume energy up to amount even if the
        /// amount can't be filled.
        /// </summary>
        /// <param name="amount">Amount requested</param>
        /// <returns>Energy consumed</returns>
        float Consume(float amount);
        
        /// <summary>
        /// Provide the net work with energy.
        /// </summary>
        /// <param name="amount">Amount to provide</param>
        /// <returns>Energy stored</returns>
        float Provide(float amount);

        
        /// <summary>
        /// Called by the energy net during updates. Resets any state for each tick.
        /// </summary>
        void Update();
    }
}