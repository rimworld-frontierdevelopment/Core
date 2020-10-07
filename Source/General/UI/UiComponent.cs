using UnityEngine;

namespace FrontierDevelopments.General.UI
{
    public interface UiComponent
    {
        int Height { get; }
        void Draw(Rect rect);
    }
}