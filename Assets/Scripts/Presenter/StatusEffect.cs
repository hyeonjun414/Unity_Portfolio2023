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

        public StatusEffect(StatusEffectModel model, StatusEffectView view)
        {
            Model = model;
            View = view;
        }

        public virtual async UniTask Activate(Character character)
        {
            await Model.Activate(character);
            await View.Activate(Model.Turn);
        }

        public virtual void Dispose(Character character)
        {
            Model.Dispose(character);
            Model = null;
            View.DestroyView();
        }
    }
}
