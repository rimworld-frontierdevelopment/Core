using Verse;

namespace FrontierDevelopments.General.UI
{
    public class Elements
    {
        public static void Heading(Listing_Standard list, string text)
        {
            list.GapLine();
            Text.Font = GameFont.Medium;
            list.Label(text);
            Text.Font = GameFont.Small;
            list.Gap();
        }
    }
}