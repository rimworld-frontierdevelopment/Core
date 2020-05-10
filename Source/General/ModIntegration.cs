using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace FrontierDevelopments.General
{
    public abstract class ModIntegration
    {
        public virtual string ThisModName { get; }
        public virtual string OtherModName { get; }
        public virtual string OtherModAssemblyName { get; }
        public virtual Version OtherModVersion { get; }
        public virtual string IntegrationAssemblyPath { get; }

        public bool TryEnable(Harmony harmony)
        {
            try
            {
                var otherAssembly = Assembly.Load(OtherModAssemblyName);
                if (otherAssembly != null)
                {
                    var version = new AssemblyName(otherAssembly.FullName).Version;
                    if (version == OtherModVersion || OtherModVersion == null)
                    {
                        var assembly = AssemblyUtility.FindModAssembly(ThisModName, IntegrationAssemblyPath);
                        if (assembly != null)
                        {
                            harmony.PatchAll(assembly);
                            Log.Message(ThisModName + " :: enabled " + OtherModName + " support for version " + version);
                            return true;
                        }
                        else
                        {
                            Log.Warning(ThisModName + " :: unable to load " + OtherModName + " support assembly");
                        }
                    }
                    else
                    {
                        Log.Warning(ThisModName + " :: " + OtherModName + " " + version + 
                                    " is loaded and " + OtherModVersion + " is required, not enabling support");
                    }
                }
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}