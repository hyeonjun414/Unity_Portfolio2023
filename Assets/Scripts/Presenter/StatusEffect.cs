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

        public virtual async UniTask Activate(Entity entity)
        {
            await Model.Activate(entity);
            await View.Activate(Model.GetTurn());
        }
    }
}
