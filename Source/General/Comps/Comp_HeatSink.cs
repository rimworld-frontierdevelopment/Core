using System;
using System.Collections.Generic;
using System.Linq;
using FrontierDevelopments.General.Energy;
using RimWorld;
using Verse;

namespace FrontierDevelopments.General.Comps
{
    public class CompProperties_HeatSink : CompProperties
    {
        public float grams;
        public float specificHeat;
        public float conductivity;

        public float minorThreshold;
        public float majorThreshold;
        public float criticalThreshold;
        public float maximumTemperature;

        public CompProperties_HeatSink()
        {
            compClass = typeof(Comp_HeatSink);
        }
    }
    
    public class Comp_HeatSink : ThingComp, IHeatsink, IFlickBoardSwitch
    {
        public static readonly float KELVIN_ZERO_CELCIUS = 273.15f;

        private float _dissipationRate;
        private float _temperature = -500f;
        private bool _thermalShutoff = true;
        private bool _wantThermalShutoff = true;
        private float _shutoffTemperature = -500f;

        public bool WantThermalShutoff
        {
            get => _wantThermalShutoff;
            set => SetThermalShutoff(value);
        }

        public bool ThermalShutoff => _thermalShutoff;

        public bool WantFlick => _wantThermalShutoff != _thermalShutoff;

        public bool CanBreakdown => !_thermalShutoff && OverMinorThreshold;
        
        public CompProperties_HeatSink Props => (CompProperties_HeatSink)props;

        public float Temp => Settings.EnableThermal ? _temperature : AmbientTemp();

        private float Joules
        {
            get => (_temperature + KELVIN_ZERO_CELCIUS) * Props.specificHeat * Props.grams;
            set
            {
                if(Settings.EnableThermal)
                    _temperature = value / Props.specificHeat / Props.grams - KELVIN_ZERO_CELCIUS;
            }
        }

        public bool OverTemperature => _thermalShutoff && Temp > _shutoffTemperature
                                       || !Settings.EnableCriticalThermalIncidents && OverMaximumTemperature;

        public bool OverMinorThreshold => Settings.EnableThermal && Temp >= Props.minorThreshold;
        
        public bool OverMajorThreshold => Settings.EnableThermal && Temp >= Props.majorThreshold;
        
        public bool OverCriticalThreshold => Settings.EnableThermal && Temp >= Props.criticalThreshold;

        public bool OverMaximumTemperature => Settings.EnableThermal && Temp >= MaximumTemperature;

        public virtual float MaximumTemperature => Props.maximumTemperature;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (_temperature < -KELVIN_ZERO_CELCIUS) _temperature = AmbientTemp();
            if (_shutoffTemperature < -KELVIN_ZERO_CELCIUS) _shutoffTemperature = Props.minorThreshold;
            _dissipationRate = GenDate.TicksPerDay / Props.conductivity;
        }

        public void PushHeat(float wattDays)
        {
            Joules += wattDays / 86.4f * 1000;
        }

        protected virtual float AmbientTemp()
        {
            return parent.AmbientTemperature;
        }

        protected virtual void DissipateHeat(float kilojoules)
        {
            try
            {
                GenTemperature.PushHeat(parent, kilojoules);
            } catch(Exception e)
            {
                // This fixes a NRE related to minify-everything
            }
        } 

        public override void CompTick()
        {
            if (OverMaximumTemperature && Settings.EnableCriticalThermalIncidents && Settings.EnableThermal)
            {
                DoCriticalBreakdown();
            }
            else
            {
                var heatDissipated =  (Temp - AmbientTemp()) / _dissipationRate;
                Joules -= heatDissipated * 1000f;
                DissipateHeat(heatDissipated);
            }
        }

        public override string CompInspectStringExtra()
        {
            LessonAutoActivator.TeachOpportunity(ConceptDef.Named("FD_CoilTemperature"), OpportunityType.Important);
            return "fd.heatsink.temperature".Translate(Temp.ToStringTemperature());
        }

        public void Notify_Flicked()
        {
            _thermalShutoff = _wantThermalShutoff;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _temperature, "temperature", -500f);
            Scribe_Values.Look(ref _thermalShutoff, "thermalShutoff", true);
            Scribe_Values.Look(ref _wantThermalShutoff, "wantThermalShutoff", true);
            Scribe_Values.Look(ref _shutoffTemperature, "shutoffTemperature", -500f);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (OwnershipUtility.PlayerOwns(parent))
            {
                yield return new Command_Toggle
                {
                    icon = Resources.UiThermalShutoff,
                    defaultDesc = "thermal_shutoff.description".Translate(),
                    defaultLabel = "thermal_shutoff.label".Translate(),
                    isActive = () => _wantThermalShutoff,
                    toggleAction = () => SetThermalShutoff(_wantThermalShutoff = !_wantThermalShutoff)
                };
            }
        }

        private void SetThermalShutoff(bool value)
        {
            _wantThermalShutoff = value;
            var flickBoard = FlickBoardUtility.FindBoard(parent);
            if (flickBoard != null)
            {
                flickBoard.Notify_Want(WantFlick);
            }
            else
            {
                _thermalShutoff = value;
            }
        }

        public void DoMinorBreakdown()
        {
            BreakdownMessage(
                "fd.shields.incident.minor.title".Translate(), 
                "fd.shields.incident.minor.body".Translate(), 
                MinorBreakdown());
        }

        public void DoMajorBreakdown()
        {
            BreakdownMessage(
                "fd.shields.incident.major.title".Translate(), 
                "fd.shields.incident.major.body".Translate(), 
                MajorBreakdown());
        }

        public void DoCriticalBreakdown()
        {
            BreakdownMessage(
                "fd.shields.incident.critical.title".Translate(), 
                "fd.shields.incident.critical.body".Translate(), 
                CriticalBreakdown());
            parent.Destroy(DestroyMode.KillFinalize);
        }
        
        private float MinorBreakdown()
        {
            var energyNet = EnergyNet.Find(parent);
            if (energyNet == null) return 0;
            var amount = energyNet.RateAvailable * (float) new Random().NextDouble();
            energyNet.Consume(amount);
            return amount;
        }

        private float MajorBreakdown()
        {
            parent.GetComp<CompBreakdownable>().DoBreakdown();
            if (OwnershipUtility.PlayerOwns(parent))
            {
                // manually remove the default letter...
                try
                {
                    Find.LetterStack.RemoveLetter(
                        Find.LetterStack.LettersListForReading
                            .First(letter => letter.lookTargets.targets.Any(t => t.Thing == parent)));
                    
                }
                catch (Exception)
                {
                }
            }
            return MinorBreakdown();
        }

        private float CriticalBreakdown()
        {
            GenExplosion.DoExplosion(
                parent.Position,
                parent.Map,
                3.5f,
                DamageDefOf.Flame,
                parent);
            return MajorBreakdown();
        }
        
        private void BreakdownMessage(string title, string body, float drained)
        {
            if (!OwnershipUtility.PlayerOwns(parent)) return;
            Find.LetterStack.ReceiveLetter(
                title,
                body.Replace("{0}", ((int)drained).ToString()), 
                LetterDefOf.NegativeEvent, 
                new TargetInfo(parent.Position, parent.Map));
        }
    }
}
