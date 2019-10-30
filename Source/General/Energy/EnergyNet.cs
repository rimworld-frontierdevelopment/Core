using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Verse;

namespace FrontierDevelopments.General.Energy
{
    public class EnergyNet : IEnergyNet, IExposable
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

        private static int NextId => Verse.Find.UniqueIDsManager.GetNextThingID();
        
        //
        //
        //

        private int _id;
        private IEnergyNet _parent;
        private bool _netPowered;
        private float _draw;

        public IEnergyNet Parent => _parent;

        private readonly HashSet<IEnergyNode> nodes = new HashSet<IEnergyNode>();

        public IEnumerable<IEnergyNode> Nodes => nodes;

        public EnergyNet()
        {
            if (Scribe.mode == LoadSaveMode.Inactive)
            {
                _id = NextId;
            }
        }

        public void Connect(IEnergyNode node)
        {
            if (node == this) return;
            nodes.Add(node);
            Changed();
        }

        public void Disconnect(IEnergyNode node)
        {
            if (node == this) return;
            nodes.Remove(node);
            Changed();
        }

        public void ConnectTo(IEnergyNet net)
        {
            _parent?.Disconnect(this);
            _parent = net;
            _parent?.Connect(this);
        }

        private float CanProvide(Func<IEnergyProvider, float> callback)
        {
            return nodes.OfType<IEnergyProvider>().Aggregate(0f, (sum, node) => sum + callback(node)) 
                   + (_netPowered && _parent != null ? callback(_parent) : 0f);
        }

        public float AmountAvailable => CanProvide(provider => provider.AmountAvailable); 

        public float RateAvailable => CanProvide(provider => provider.RateAvailable); 

        public float TotalAvailable => CanProvide(provider => provider.TotalAvailable);

        public float MaxRate => CanProvide(provider => provider.MaxRate);

        private float HandleEnergy(float amount, Func<IEnergyProvider, float, float> callback)
        {
            if (amount < 0) throw new InvalidOperationException("Can't provide a negative amount");

            var remaining = amount;

            foreach (var node in nodes.OfType<IEnergyProvider>())
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
            var result = HandleEnergy(amount, (node, remaining) => node.Consume(remaining));
            if (result < amount) _draw += amount - result;
            return result;
        }

        public void Update()
        {
            _draw = 0;
            nodes.OfType<IEnergyProvider>().Do(provider => provider.Update());
            nodes.OfType<IEnergyConsumer>().Do(consumer =>
            {
                if (RateAvailable - consumer.Rate < 0)
                {
                    consumer.HasPower(false);
                }
                else
                {
                    consumer.HasPower(true);
                    Consume(consumer.Rate);
                }
            });
        }

        public void Changed()
        {
            if (_parent != null) _parent.Changed();
            else Update();
        }

        public float Rate => _draw;
        
        public void HasPower(bool isPowered)
        {
            var last = _netPowered;
            _netPowered = isPowered;
            if (_netPowered != last) Update();
        }

        public string GetUniqueLoadID()
        {
            return "EnergyNet" + _id;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref _id, "id");
            Scribe_References.Look(ref _parent, "parent");
            Scribe_Values.Look(ref _netPowered, "netPowered");
            Scribe_Values.Look(ref _draw, "draw");
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ConnectTo(_parent);
            }
        }
    }
}