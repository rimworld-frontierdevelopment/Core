using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FrontierDevelopments.General
{
    public interface IHeatsink
    {
        bool OverTemperature { get; }
        float Temp { get; }
        void PushHeat(float wattDays);
    }

    public class HeatsinkUtility
    {
        public static IHeatsink Find(ThingWithComps parent)
        {
            switch (parent)
            {
                case IHeatsink parentHeatsink:
                    return parentHeatsink;
                default:
                    return FindComp(parent.AllComps);
            }
        }

        public static IHeatsink FindComp(IEnumerable<ThingComp> comps)
        {
            try
            {
                return comps.OfType<IHeatsink>().First();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}