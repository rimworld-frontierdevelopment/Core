﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace FrontierDevelopments.General.Comps
{
    public class CompProperties_BatteryInternal : CompProperties_Battery
    {
        public float chargeRate;
        
        public CompProperties_BatteryInternal()
        {
            compClass = typeof(Comp_BatteryInternal);
        }
    }
    
    public class Comp_BatteryInternal : CompPowerBattery
    {
        private CompPowerTrader _powerTrader;
        
        public new CompProperties_BatteryInternal Props => (CompProperties_BatteryInternal)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _powerTrader = parent.GetComp<CompPowerTrader>();
        } 
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var current in base.CompGetGizmosExtra())
                yield return current;

            if (OwnershipUtility.PlayerOwns(parent))
            {
                yield return new Command_Action
                {
//                    icon = Resources.UiChargeBattery,
                    defaultDesc = "charge_internal_battery.description".Translate(),
                    defaultLabel = "charge_internal_battery.label".Translate(),
                    activateSound = SoundDef.Named("Click"),
                    action =  ChargeInternalBattery
                };
            }
        }
        
        public override string CompInspectStringExtra()
        {
            return "PowerBatteryStored".Translate() + ": " + StoredEnergy.ToString("F0") + " / " + Props.storedEnergyMax.ToString("F0") + " Wd" + ", " + "PowerBatteryEfficiency".Translate() + ": " + (Props.efficiency * 100f).ToString("F0") + "%";
        }

        private void ChargeFromOther(CompPowerBattery battery, float charge)
        {
            battery.DrawPower(charge);
            var efficiency = Props.efficiency;
            Props.efficiency = 1.0f;
            AddEnergy(charge);
            Props.efficiency = efficiency;
        }

        private void ChargeInternalBattery()
        {
            if (_powerTrader.PowerNet == null) return;
            foreach (var battery in _powerTrader.PowerNet.batteryComps.InRandomOrder().Where(b => b != this))
            {
                var deficit = Props.storedEnergyMax - StoredEnergy;
                var drain = deficit / Props.efficiency;
                if (drain < battery.StoredEnergy)
                {
                    ChargeFromOther(battery, drain);
                    break;
                }
                ChargeFromOther(battery, battery.StoredEnergy);
            }
        }

        // weird hack to prevent double serialization of <parentThing> since this has two CompPower
        public override void PostExposeData()
        {
            var storedEnergy = StoredEnergy;
            Scribe_Values.Look<float>(ref storedEnergy, "storedPower", 0.0f, false);
            
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                var efficiency = Props.efficiency;
                Props.efficiency = 1.0f;
                AddEnergy(storedEnergy);
                Props.efficiency = efficiency;
            }
        }
    }
}