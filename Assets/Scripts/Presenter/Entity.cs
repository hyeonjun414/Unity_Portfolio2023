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
            View.Init(this);
        }

        public void AddAp(float deltaTime)
        {
            Model.AddAp(deltaTime);
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

        public void Dispose()
        {
            Model = null;
            View.DestroyView();
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
            foreach (var statEft in StatusEffects)
            {
                await statEft.Activate(this);
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
            var task = SetAction();
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

        public async UniTask SetAction()
        {
            if(IsExecutable())
                eModel.SetAction();
            await eView.SetActionView(eModel.GetCurAction());
        }

        public async UniTask ExecuteAction(Hero hero)
        {
            var curAct = eModel.GetCurAction();
            if(IsExecutable())
                await curAct.Activate(this, hero);
            else
            {
                curAct.Turn--;
                await SetAction();
                await eView.Wait();
            }
        }

        public bool IsExecutable()
        {
            return eModel.GetCurAction().Turn <= 1;
        }
    }

    public class Hero : Entity
    {
        public HeroModel hModel => Model as HeroModel;
        public HeroView hView => View as HeroView;
        public Hero(HeroModel model, EntityView view) : base(model, view)
        {
        }

        
    }
}
