using System;
using UnityEngine;
using Verse;

namespace FrontierDevelopments.General.UI
{
    public class CheckboxComponent : UiComponent
    {
        private readonly string _label;
        private readonly Func<bool> _current;
        private readonly Action<bool> _onSet;

        public CheckboxComponent(string label, Func<bool> current, Action<bool> onSet)
        {
            _label = label;
            _current = current;
            _onSet = onSet;
        }

        public int Height => _label.Split('\n').Length * 24;

        public void Draw(Rect rect)
        {
            var last = _current();
            var current = last;

            Widgets.CheckboxLabeled(rect, _label, ref current);

            if (current != last)
                _onSet(current);
        }
    }
}