using System;
using UnityEngine;
using Verse;

namespace FrontierDevelopments.General.UI
{
    public class IntSlider : UiComponent
    {
        private readonly string _label;
        private readonly int _floor;
        private readonly int _ceiling;
        private readonly Func<int> _current;
        private readonly Action<int> _onSet;

        public IntSlider(string label, int floor, int ceiling, Func<int> current, Action<int> onSet)
        {
            _label = label;
            _floor = floor;
            _ceiling = ceiling;
            _current = current;
            _onSet = onSet;
        }

        public void Draw(Rect rect)
        {
            _onSet((int)Widgets.HorizontalSlider(
                rect, 
                _current(), 
                _floor, 
                _ceiling, 
                false, 
                "" + _current() + "/" + _ceiling, 
                _label));
        }
    }
}