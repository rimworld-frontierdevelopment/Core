using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace FrontierDevelopments.General.Comps
{
    public class CompProperties_FlickBoard : CompProperties
    {
        public CompProperties_FlickBoard()
        {
            compClass = typeof(Comp_FlickBoard);
        }
    }

    public interface IFlickBoardSwitch
    {
        bool WantFlick { get; }
        void Notify_Flicked();
    }

    public class Comp_FlickBoard : ThingComp
    {
        private static bool JobPatched = true;

        private IEnumerable<IFlickBoardSwitch> Switches => FlickBoardUtility.FindSwitches(parent);

        private bool WantFlick => Switches.Any(flickSwitch => flickSwitch.WantFlick);

        private void FlickSwitches() => Switches.Do(flickSwitch => flickSwitch.Notify_Flicked());

        private void DoFlick()
        {
            if (WantFlick)
            {
                FlickSwitches();
                SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
            }
        }

        private void UpdateDesignation()
        {
            if (!JobPatched)
            {
                FlickSwitches();
                return;
            }
            
            var designationManager = parent?.Map?.designationManager;
            if (designationManager != null)
            {
                var flickDesignation = parent.Map.designationManager.DesignationOn(parent, DesignationDefOf.Flick);
                if (WantFlick)
                {
                    if(flickDesignation == null)
                        designationManager.AddDesignation(new Designation((LocalTargetInfo) parent, DesignationDefOf.Flick));
                }
                else
                {
                    flickDesignation?.Delete();
                }
                TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.SwitchFlickingDesignation, Array.Empty<string>());
            }
        }

        public void Notify_Want(bool wantFlick)
        {
            UpdateDesignation();
        }

        public override void PostExposeData()
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // update designation if needed here
            }
        }
        
        private static void HandleFlickBoardFlick(ThingComp comp)
        {
            switch (comp)
            {
                case Comp_FlickBoard flickBoard:
                    if(flickBoard.WantFlick)
                        flickBoard.DoFlick();
                    break;
            }
        }

        [HarmonyPatch]
        private static class Patch_MakeNewToils
        {
            [HarmonyTargetMethod]
            private static MethodInfo FindMakeNewToils()
            {
                return typeof(JobDriver_Flick)
                    .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                    .SelectMany(AccessTools.GetDeclaredMethods)
                    .Where(method => method.ReturnType == typeof(void))
                    .Where(method => method.GetParameters().Length == 0)
                    .First(method => method.Name.Contains("MakeNewToils"));
            }

            private class Patcher
            {
                private bool addedFlickBoardHandling;
                
                private IEnumerable<CodeInstruction> AddHandleFlickBoardFlick(IEnumerable<CodeInstruction> instructions)
                {
                    foreach (var instruction in instructions)
                    {
                        if (instruction.opcode == OpCodes.Isinst
                            && (Type)instruction.operand == typeof(CompFlickable))
                        {
                            addedFlickBoardHandling = true;
                            
                            yield return new CodeInstruction(OpCodes.Dup);
                            yield return new CodeInstruction(
                                OpCodes.Call,
                                AccessTools.Method(
                                    typeof(Comp_FlickBoard),
                                    nameof(HandleFlickBoardFlick)));
                        }

                        yield return instruction;
                    }
                }

                public IEnumerable<CodeInstruction> Apply(IEnumerable<CodeInstruction> instructions)
                {
                    var original = instructions.ToList();
                    var patched = AddHandleFlickBoardFlick(original).ToList();

                    if (addedFlickBoardHandling)
                    {
                        return patched;
                    }

                    Log.Warning("FrontierDevelopments Core :: Unable to add flickboard job. Flicks will not be required");
                    JobPatched = false;
                    return original;
                }
            }

            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> AddFlickBoardHandling(IEnumerable<CodeInstruction> instructions)
            {
                return new Patcher().Apply(instructions);
            }
        }
    }

    public static class FlickBoardUtility
    {
        public static Comp_FlickBoard FindBoard(ThingWithComps thing)
        {
            return thing.GetComp<Comp_FlickBoard>();
        }

        public static IEnumerable<IFlickBoardSwitch> FindSwitches(Thing thing)
        {
            if (thing is IFlickBoardSwitch thingSwitch)
            {
                yield return thingSwitch;
            }

            if (thing is ThingWithComps thingWithComps)
            {
                foreach (var compSwitch in Find(thingWithComps.AllComps))
                {
                    yield return compSwitch;
                }
            }
        }

        public static IEnumerable<IFlickBoardSwitch> Find(IEnumerable<ThingComp> comps)
        {
            return comps.OfType<IFlickBoardSwitch>();
        }
    }
}