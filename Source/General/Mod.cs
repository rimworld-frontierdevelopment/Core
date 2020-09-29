using System.Reflection;
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
}