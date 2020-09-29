using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FrontierDevelopments.General.Comps
{
    public class CompProperties_FlickBoard : CompProperties
    {
        public CompProperties_FlickBoard()
        {
            compClass = typeof(Comp_FlickBoard);
        }
    }

    public class Comp_FlickBoard : ThingComp
    {
        private static bool JobPatched = true;
        
        public const string SignalFlicked = "FlickBoard_Flicked";
        public const string SignalWant = "FlickBoard_Want";
        public const string SignalReset = "FlickBoard_Reset";
        
        private bool _wantFlick;

        public bool WantFlick
        {
            get => _wantFlick;
            set
            {
                _wantFlick = value;
                UpdateDesignation();
            }
        }

        private void DoFlick()
        {
            if (WantFlick)
            {
                WantFlick = false;
                parent.BroadcastCompSignal(SignalFlicked);
            }
        }

        private void UpdateDesignation()
        {
            if (!JobPatched)
            {
                parent.BroadcastCompSignal(SignalFlicked);
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

        public override void ReceiveCompSignal(string signal)
        {
            switch (signal)
            {
                case SignalWant:
                    WantFlick = true;
                    break;
                case SignalReset:
                    WantFlick = false;
                    break;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _wantFlick, "wantFlick");
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
}