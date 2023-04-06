using System.Collections.Generic;
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

        public List<StatusEffect> StatusEffects;

        public Entity(EntityModel model, EntityView view)
        {
            Model = model;
            View = view;
            StatusEffects = new List<StatusEffect>();
        }

        public virtual void Init()
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
            Model.AddActionGauge();
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

        public virtual async UniTask AddStatusEffect(StatusEffectModel statEft)
        {
            var eftPresenter = new StatusEffect(statEft, null);
            StatusEffects.Add(eftPresenter);
            Model.StatusEffects.Add(statEft);
            await View.AddStatusEffect(eftPresenter);
        }

        public virtual async UniTask StatusEffectActivate()
        {
            if (Model.IsActionCountUp)
            {
                Model.IsActionCountUp = false;
                foreach (var statEft in StatusEffects)
                {
                    await statEft.Activate(this);
                }
            }
        }
    }

    public class Enemy : Entity
    {
        public EnemyModel eModel => Model as EnemyModel;
        public EnemyView eView => View as EnemyView;
        public Enemy(EnemyModel model, EntityView view) : base(model, view)
        {
            Debug.Log("Enemy Gen");
        }

        public override void Init()
        {
            base.Init();
            SetAction();
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
            eModel.SetAction();
            eView.SetActionView(eModel.GetCurAction());
        }

        public async UniTask ExecuteAction(Hero hero)
        {
            var curAct = eModel.GetCurAction();
            UseActionCount(curAct.Cost);
            await curAct.Activate(this, hero);
            SetAction();
        }


        public bool IsActExecutable()
        {
            return eModel.ActionCount >= eModel.GetCurAction().Cost;
        }
    }

    public class Hero : Entity
    {
        public HeroModel hModel => Model as HeroModel;
        public HeroView hView => View as HeroView;
        public Hero(HeroModel model, EntityView view) : base(model, view)
        {
        }

        public bool CanDrawCard()
        {
            return Model.ActionCount >= 1;
        }
    }
}
