using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class EntityPresenter
    {
        public EntityModel Model;
        public EntityView View;

        public EntityPresenter(EntityModel model, EntityView view)
        {
            Model = model;
            View = view;
        }

        public void UpdateEntityInfo()
        {
            View.UpdateHp(Model.CurHp, Model.MaxHp);
            View.UpdateActionGauge(Model.CurActionGauge, Model.MaxActionGauge);
        }

        public async UniTask TakeDamage(float damage)
        {
            Model.TakeDamage(damage);
            View.PlayDamageEft();
            View.UpdateHp(Model.CurHp, Model.MaxHp);
            await UniTask.Yield();
        }

        public async UniTask PrepareAttack(Vector3 targetPos)
        {
            Model.IsActionReady = false;
            Model.CurActionGauge = 0;
            View.UpdateActionGauge(Model.CurActionGauge, Model.MaxActionGauge);
            await View.PrepareAttack(targetPos);
        }

        public async UniTask PlayAttack()
        {
            await View.PlayAttack();
        }

        public async UniTask EndAttack(Vector3 targetPos)
        {
            await View.EndAttack(targetPos);
        }

        public void AddActionGauge()
        {
            Model.AddActionGauge();
            View.UpdateActionGauge(Model.CurActionGauge, Model.MaxActionGauge);
        }
    }

    public class EnemyPresenter : EntityPresenter
    {
        public EnemyPresenter(EntityModel model, EntityView view) : base(model, view)
        {
        }
    }
}
