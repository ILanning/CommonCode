using System;

namespace CommonCode.UI
{
    public interface IClickable
    {
        event EventHandler Selected;
        event EventHandler Clicked;
        event EventHandler Collided;

        bool OnCollision();
        bool OnClicked();
        void OnSelectItem();
    }
}
