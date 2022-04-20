using System.Reflection;
using UnityEngine;
using Verse;

namespace FrontierDevelopments.General
{
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            var harmony = new HarmonyLib.Harmony("FrontierDevelopments.Core");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [StaticConstructorOnStartup]
    public static class Resources
    {
        public static readonly Texture2D UiThermalShutoff = ContentFinder<Texture2D>.Get("UI/Buttons/ThermalShutoff");
    }
}