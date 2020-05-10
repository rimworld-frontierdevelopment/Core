using RimWorld;
using Verse;

namespace FrontierDevelopments.General
{
    public class OwnershipUtility
    {
        public static bool PlayerOwns(Thing thing)
        {
            if (thing.Faction == Faction.OfPlayer)
            {
                return true;
            }
            return PlayerOwns(thing.ParentHolder);
        }

        public static bool PlayerOwns(IThingHolder holder)
        {
            switch (holder.ParentHolder)
            {
                case null:
                    return false;

                case MinifiedThing minifiedThing:
                    return PlayerOwns(minifiedThing.ParentHolder);

                case Pawn pawn:
                    return pawn.Faction == Faction.OfPlayer;

                case Pawn_InventoryTracker inventory:
                    return inventory.pawn.Faction == Faction.OfPlayer;

                case IThingHolder parent:
                    return PlayerOwns(parent);
            }
        }

        public static Faction GetFaction(Thing thing)
        {
            return GetFaction(thing.ParentHolder);
        }

        public static Faction GetFaction(IThingHolder holder)
        {
            switch (holder.ParentHolder)
            {
                case null:
                    return null;

                case MinifiedThing minifiedThing:
                    return GetFaction(minifiedThing.ParentHolder);

                case Pawn pawn:
                    return pawn.Faction;

                case Pawn_InventoryTracker inventory:
                    return inventory.pawn.Faction;

                case IThingHolder parent:
                    return GetFaction(parent);
            }
        }
    }
}