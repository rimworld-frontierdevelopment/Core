using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace FrontierDevelopments.General.UI
{
    public class TextComponent : UiComponent
    {
        private readonly List<string> _lines;
        
        public TextComponent(List<string> lines)
        {
            _lines = lines;
        }
        
        public TextComponent(string value)
        {
            _lines = new List<string>(value.Split('\n'));
        }

        public TextComponent(StringBuilder text)
        {
            _lines = new List<string>(text.ToString().TrimEndNewlines().Split('\n'));
        }

        public int Height => _lines.Count * 24;

        public void Draw(Rect rect)
        {
            var list = new Listing_Standard();
            list.Begin(rect);
            
            foreach (var line in _lines)
            {
                list.Label(line);
            }
            list.End();
        }
    }
}