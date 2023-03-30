using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class Entity
    {
        public EntityModel Model;
        public EntityView View;

        public Entity(EntityModel model, EntityView view)
        {
            Model = model;
            View = view;
        }

        public void Init()
        {
            View.Presenter = this;
            View.Init(Model);
        }

        public bool GetActionReady()
        {
            return Model.IsActionReady;
        }
        
        public async UniTask TakeDamage(float damage)
        {
            Model.TakeDamage(damage);
            var stage = GameManager.Instance.CurStage as BattleStage;
            stage?.CreateFloatingText(((int)damage).ToString(), View.transform.position);
            if (Model.IsDead)
                await View.Dead();
            else
                View.PlayDamageEft();
            
            View.UpdateHp(Model.CurHp, Model.MaxHp);
            await UniTask.Yield();
        }

        public async UniTask PrepareAttack(Vector3 targetPos)
        {
            //Model.IsActionReady = false;
            //Model.CurActionGauge = 0;
            View.UpdateActionGauge(Model.CurActionGauge, Model.MaxActionGauge, Model.ActionCount);
            await View.PrepareAttack(targetPos);
        }

        public async UniTask PlayAttack()
        {
            await View.PlayAttack();
        }

        public async UniTask EndAttack()
        {
            await View.EndAttack();
        }

        public async UniTask AddActionGauge()
        {
            Model.AddActionGauge(out var isActionUp);
            if (isActionUp)
                await View.StatusEffectActivate(Model.StatusEffects);
            View.UpdateActionGauge(Model.CurActionGauge, Model.MaxActionGauge, Model.ActionCount);
        }

        public void Dispose()
        {
            Model = null;
            View.DestroyView();
        }

        public void UseActionCount(int cost)
        {
            Model.ActionCount -= cost;
            View.UpdateActionGauge(Model.CurActionGauge, Model.MaxActionGauge, Model.ActionCount);
        }

        public int GetActionCount()
        {
            return Model.ActionCount;
        }

        public async UniTask AddStatusEffect(StatusEffectModel effectModel)
        {
            Model.AddStatusEffect(effectModel);
            await View.AddStatusEffect(effectModel);
        }
    }

    public class Enemy : Entity
    {
        public Enemy(EnemyModel model, EntityView view) : base(model, view)
        {
        }

        public void Targeted()
        {
            if (Model.IsDead) return;
            var stage = GameManager.Instance.CurStage as BattleStage;
            stage?.TargetEnemy(this);
        }

        public void UnTargeted()
        {
            if (Model.IsDead) return;
            var stage = GameManager.Instance.CurStage as BattleStage;
            stage?.UnTargetEnemy(this);
        }

        public void SetAction()
        {
            if (Model is EnemyModel em)
            {
                em.SetAction();
                ((EnemyView)View).SetActionView(em.GetCurAction());
            }
        }

        public async UniTask ExecuteAction(Hero hero)
        {
            if (Model is EnemyModel em)
            {

                var curAct = em.GetCurAction();
                UseActionCount(curAct.Cost);
                await curAct.Activate(this, hero);
                SetAction();
            }
        }


        public bool IsActExecutable()
        {
            if (Model is EnemyModel em)
            {
                return em.ActionCount >= em.GetCurAction().Cost;
            }

            return false;
        }
    }

    public class Hero : Entity
    {
        public Hero(HeroModel model, EntityView view) : base(model, view)
        {
        }

        public bool CanDrawCard()
        {
            return Model.ActionCount >= 1;
        }
    }
}
