using System;
using FrontierDevelopments.General.UI;
using UnityEngine;
using Verse;

namespace FrontierDevelopments.General.Windows
{
    public class Popup_IntSlider : Window
    {
        private IntSlider _intSlider;

        private int _width;
        private int _height;

        public Popup_IntSlider(string label, int floor, int ceiling, Func<int> current, Action<int> callback, int width = 215, int height = 75)
        {
            _intSlider = new IntSlider(label, floor, ceiling, current, callback);
            _width = width;
            _height = height;
        }

        protected override void SetInitialSizeAndPosition()
        {
            var vector = Verse.UI.MousePositionOnUIInverted;
            if (vector.x + InitialSize.x > Verse.UI.screenWidth)
            {
                vector.x = Verse.UI.screenWidth - InitialSize.x;
            }
            if (vector.y + InitialSize.y - 50 > Verse.UI.screenHeight)
            {
                vector.y = Verse.UI.screenHeight - InitialSize.y - 50;
            }
            windowRect = new Rect(vector.x, vector.y + InitialSize.y - 100, _width, _height);
        }

        public override void DoWindowContents(Rect rect)
        {
            if (!rect.Contains(Event.current.mousePosition))
            {
                var num = GenUI.DistFromRect(rect, Event.current.mousePosition);
                if (num > 75f)
                {
                    Close(false);
                    return;
                }
            }
            _intSlider.Draw(new Rect(5, 10, _width - 50, _height - 50));
        }
    }
}