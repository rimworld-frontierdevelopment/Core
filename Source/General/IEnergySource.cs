using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FrontierDevelopments.General
{
    public interface IEnergySource
    {
        bool IsActive();
        bool WantActive { get; }
        float BaseConsumption { get; set; }
        float EnergyAvailable { get; }
        float Draw(float amount);
        void Drain(float amount);
    }

    public class EnergySourceUtility
    {
        public static IEnergySource Find(ThingWithComps parent)
        {
            switch (parent)
            {
                case IEnergySource parentSource:
                    return parentSource;
                default:
                    return FindComp(parent.AllComps);
            }
        }

        public static IEnergySource FindComp(IEnumerable<ThingComp> comps)
        {
            try
            {
                return comps.OfType<IEnergySource>().First();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }   
}
