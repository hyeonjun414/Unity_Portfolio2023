
using System;
using Model;
using View;

namespace Presenter
{
    public class Chest
    {
        public ChestModel Model;
        public ChestView View;

        public Action OnClickAction;

        public Chest(ChestModel model)
        {
            Model = model;
        }
        
        public void SetView(ChestView view)
        {
            View = view;
            View.Presenter = this;
        }

        public void OnClick()
        {
            OnClickAction?.Invoke();
        }
    }
}
