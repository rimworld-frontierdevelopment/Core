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
        private readonly Action<bool> _hasMouse;

        public IntSlider(string label, int floor, int ceiling, Func<int> current, Action<int> onSet)
        {
            _label = label;
            _floor = floor;
            _ceiling = ceiling;
            _current = current;
            _onSet = onSet;
            _hasMouse = null;
        }

        public IntSlider(string label, int floor, int ceiling, Func<int> current, Action<int> onSet, Action<bool> hasMouse)
        {
            _label = label;
            _floor = floor;
            _ceiling = ceiling;
            _current = current;
            _onSet = onSet;
            _hasMouse = hasMouse;
        }

        public int Height => 60;

        public void Draw(Rect rect)
        {
            _hasMouse?.Invoke(rect.Contains(Event.current.mousePosition));
            var current = _current();
            var result = (int) Widgets.HorizontalSlider(
                rect,
                _current(),
                _floor,
                _ceiling,
                false,
                "" + current + "/" + _ceiling,
                _label,
                roundTo: 1f);
            if(result != current)
                _onSet(result);
        }
    }
}