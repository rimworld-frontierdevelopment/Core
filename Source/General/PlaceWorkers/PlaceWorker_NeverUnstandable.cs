using System.Linq;
using Verse;

namespace FrontierDevelopments.General.PlaceWorkers
{
    public class PlaceWorker_NeverUnstandable : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            return !GenAdj.OccupiedRect(center, rot, def.Size).Cells.Any(cell =>
                map.thingGrid.ThingsAt(cell)
                    .Where(t => t != thingToIgnore)
                    .Any(t => t.def.passability == Traversability.Impassable));
        }
    }
}