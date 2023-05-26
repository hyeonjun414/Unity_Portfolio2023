using System;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class Door
    {
        public DoorModel Model;
        public DoorView View;

        public Action OnClickEvent;
        
        public Door(DoorModel model)
        {
            Model = model;
        }

        public void SetView(DoorView view)
        {
            View = view;
            View.Presenter = this;
        }

        public void OnClick()
        {
            OnClickEvent?.Invoke();
        }
    }
}
