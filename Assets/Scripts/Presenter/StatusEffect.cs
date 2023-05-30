using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class StatusEffect
    {
        public StatusEffectModel Model;
        public StatusEffectView View;

        public StatusEffect(StatusEffectModel model)
        {
            Model = model;
        }

        public void Init(Character character)
        {
            Model.Init(character);
        }
        
        public void SetView(StatusEffectView view)
        {
            View = view;
            View.Presenter = this;
            View.SetView();
        }

        public virtual async UniTask Activate(Character character)
        {
            await Model.Activate(character);
            await View.Activate(character, Model.Turn);
        }

        public virtual void Dispose(Character character)
        {
            Model.Dispose(character);
            Model = null;
            View.DestroyView();
        }

        
    }
}
