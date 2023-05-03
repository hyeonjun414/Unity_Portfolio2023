using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public interface IEntity
    {
        void AddObserver(IEntityObserver observer);
        void RemoveObserver(IEntityObserver observer);
    }
    public class Character : IEntity
    {
        public CharacterModel Model;
        public CharacterView View;

        public List<StatusEffect> StatusEffects;

        public event EventHandler OnDeath;

        public Character(CharacterModel model, CharacterView view)
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
            stage?.CreateFloatingText(((int)damage).ToString(), View.transform.position, TextType.Damage);
            if (Model.IsDead)
            {
                OnDeathEvent();
                //await View.Dead();
            }
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

        public virtual async UniTask ExecuteAction(Character target)
        {
            await UniTask.Yield();
        }

        public virtual async UniTask StatusEffectActivate()
        {
            var expiredEffect = new List<StatusEffect>();
            foreach (var statEft in StatusEffects)
            {
                await statEft.Activate(this);
                if (statEft.Model.Turn <= 0)
                {
                    expiredEffect.Add(statEft);
                }
            }

            foreach (var expired in expiredEffect)
            {
                expired.Dispose();
                StatusEffects.Remove(expired);
            }
        }

        public async UniTask UseAp(float value)
        {
            Model.UseAp(value);
            await UniTask.Yield();
        }

        public async UniTask HpRecover(float value)
        {
            Model.HpRecover(value);
            await View.HpRecover(Model.CurHp, Model.MaxHp);
        }

        public virtual void Targeted()
        {
            if (Model.IsDead) return;
            var stage = GameManager.Instance.CurStage as BattleStage;
            stage?.TargetEntity(this);
        }

        public virtual void UnTargeted()
        {
            if (Model.IsDead) return;
            var stage = GameManager.Instance.CurStage as BattleStage;
            stage?.UnTargetEntity();
        }

        public void AddObserver(IEntityObserver observer)
        {
            View.Observers.Add(observer);
        }

        public void RemoveObserver(IEntityObserver observer)
        {
            View.Observers.Remove(observer);
        }

        public virtual void OnDeathEvent()
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    public class Enemy : Character
    {
        public EnemyModel eModel => Model as EnemyModel;
        public EnemyView eView => View as EnemyView;
        public Enemy(EnemyModel model, CharacterView view) : base(model, view)
        {
            Debug.Log("Enemy Gen");
        }

        public override void Init()
        {
            base.Init();
            var task = SetAction();
            
        }

        public async UniTask SetAction()
        {
            if(IsExecutable())
                eModel.SetAction();
            await eView.SetActionView(eModel.GetCurAction());
        }

        public override async UniTask ExecuteAction(Character target)
        {
            var curAct = eModel.GetCurAction();
            Model.UseAp();
            if (IsExecutable())
            {
                await curAct.Activate(this, target);
                await SetAction();
            }

            else
            {
                curAct.Turn--;
                await eView.SetActionView(curAct);
                await eView.Wait();
            }
        }

        public bool IsExecutable()
        {
            return eModel.GetCurAction().Turn <= 1;
        }
    }

    public class Ally : Character
    {
        public AllyModel aModel => Model as AllyModel;
        public AllyView aView => View as AllyView;

        public Ally(AllyModel model, CharacterView view) : base(model, view)
        {
            Debug.Log("Ally Gen");
        }

        public override void Init()
        {
            base.Init();
            var task = SetAction();
        }

        public async UniTask SetAction()
        {
            if (IsExecutable())
                aModel.SetAction();
            await aView.SetActionView(aModel.GetCurAction());
        }

        public override async UniTask ExecuteAction(Character target)
        {
            var curAct = aModel.GetCurAction();
            Model.UseAp();
            if (IsExecutable())
            {
                await curAct.Activate(this, target);
                await SetAction();
            }

            else
            {
                curAct.Turn--;
                await aView.SetActionView(curAct);
                await aView.Wait();
            }
            if (Model.IsDead)
            {
                OnDeathEvent();
            }
        }

        public bool IsExecutable()
        {
            return aModel.GetCurAction().Turn <= 1;
        }
    }

    public class Hero : Character
    {
        public HeroModel hModel => Model as HeroModel;
        public HeroView hView => View as HeroView;
        public Hero(HeroModel model, CharacterView view) : base(model, view)
        {
        }

        
    }
}
