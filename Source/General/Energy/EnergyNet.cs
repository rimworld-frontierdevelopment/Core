using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FrontierDevelopments.General.Energy
{
    public class EnergyNet : IEnergyNet, IEnergyNode
    {
        public static IEnergyNet Find(Thing parent)
        {
            switch (parent)
            {
                case IEnergyNet parentSource:
                    return parentSource;
                case ThingWithComps thingWithComps:
                    return FindComp(thingWithComps.AllComps);
                default:
                    return null;
            }
        }

        public static IEnergyNet FindComp(IEnumerable<ThingComp> comps)
        {
            try
            {
                return comps.OfType<IEnergyNet>().First();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        
        //
        //
        //
        
        private readonly HashSet<IEnergyNode> nodes = new HashSet<IEnergyNode>();

        public void Connect(IEnergyNode node)
        {
            nodes.Add(node);
        }

        public void Disconnect(IEnergyNode node)
        {
            nodes.Remove(node);
        }

        public float AmountAvailable => nodes.Aggregate(0f, (sum, node) => sum + node.AmountAvailable);
        
        public float RateAvailable => nodes.Aggregate(0f, (sum, node) => sum + node.RateAvailable);

        public float TotalAvailable => nodes.Aggregate(0f, (sum, node) => sum + node.TotalAvailable);

        public float MaxRate => nodes.Aggregate(0f, (sum, node) => sum + node.MaxRate);

        private float HandleEnergy(float amount, Func<IEnergyNode, float, float> callback)
        {
            if (amount < 0) throw new InvalidOperationException("Can't provide a negative amount");

            var remaining = amount;

            foreach (var node in nodes)
            {
                if (remaining <= 0) break;
                var handled = callback(node, remaining);

                remaining -= handled;
            }

            return amount - remaining;
        }
        
        public float Provide(float amount)
        {
            return HandleEnergy(amount, (node, remaining) => node.Provide(remaining));
        }

        public float Consume(float amount)
        {
            return HandleEnergy(amount, (node, remaining) => node.Consume(remaining));
        }
    }
}