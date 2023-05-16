using System;

namespace Presenter
{
    public class Item
    {
        public EventHandler OnSell;

        public void OnSellEvent()
        {
            OnSell?.Invoke(this, EventArgs.Empty);
        }
    }
}
