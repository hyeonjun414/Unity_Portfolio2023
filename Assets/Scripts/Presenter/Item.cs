using System;
using Newtonsoft.Json;

namespace Presenter
{
    public class Item
    {
        [JsonIgnore]
        public ItemState State;

        public void SetState()
        {
            State = new ItemState();
            State.EnterState();
        }
        public void OnClick() { State.OnClick(); }
        public void OnHover() { State.OnHover(); }
        public void OnUnhover() { State.OnUnhover(); }
        public void OnClickDown() { State.OnClickDown(); }
        public void OnClickUp() { State.OnClickUp(); }
    }
}
