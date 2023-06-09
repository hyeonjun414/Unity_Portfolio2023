using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using Newtonsoft.Json;
using UnityEngine;
using View;

namespace Presenter
{
    public interface ICharacter
    {
        void AddObserver(ICharacterObserver observer);
        void RemoveObserver(ICharacterObserver observer);
    }
    public class Character : ICharacter
    {
        public CharacterModel Model;

        [JsonIgnore]
        public CharacterView View;

        public List<StatusEffect> StatusEffects = new();
        private List<StatusEffect> _expiredEffect = new();

        public Action OnDeath;

        [JsonIgnore] 
        public Vector3 WorldPosition => View.GetPosition();
        [JsonIgnore] 
        public Vector3 CenterPos => View.CenterPos;
        [JsonIgnore] 
        public Transform CenterPivot => View.centerPivot;
        public CharacterType CharType => Model.CharType;


        public Character(CharacterModel model)
        {
            Model = model;
            Init();
        }

        public virtual void Init()
        {
        }

        public virtual void SetView(CharacterView view)
        {
            View = view;
            View.Presenter = this;
            View.SetView(this);
        }

        public virtual void AddTag(StatTag tag, float value)
        {
            Model.AddTag(tag, value);
        }

        public virtual void RemoveTag(StatTag tag, float value)
        {
            Model.RemoveTag(tag, value);
        }

        public virtual bool FindTag(StatTag tag, out float tagValue)
        {
            return Model.StatTags.TryGetValue(tag, out tagValue);
        }

        public void AddAp(float deltaTime)
        {
            Model.AddAp(deltaTime);
        }

        public async UniTask Attack(Character target, int damage)
        {
            if (Model.HitRate >= GameManager.Instance.Rand.Range(0, 1f))
            {
                if (target.FindTag(StatTag.Weak, out var weakDamage))
                {
                    damage += (int)weakDamage;
                }
                
                await target.TakeDamage(damage);
                if (target.FindTag(StatTag.Reflect, out var reflectDamage))
                {
                    await TakeDamage(reflectDamage);
                }
            }
            else
            {
                GameManager.Instance.CreateFloatingText("MISS", target.CenterPos, TextType.Damage);
            }
        }

        public virtual async UniTask TakeDamage(float damage)
        {
            if (Model.IsDead) return;
            Model.TakeDamage(damage);
            View.SetDefence(Model.Defence);
            GameManager.Instance.CreateFloatingText(((int)damage).ToString(), CenterPos, TextType.Damage);
            if (Model.IsDead)
            {
                OnDeathEvent();
            }
            await View.PlayDamageEft();
            View.UpdateHp(Model.CurHp, Model.MaxHp);
            await UniTask.Yield();
        }

        public virtual async UniTask TakeDamageByItem(float damage)
        {
            if (Model.IsDead) return;
            Model.TakeDamage(damage);
            View.SetDefence(Model.Defence);
            GameManager.Instance.CreateFloatingText(((int)damage).ToString(), CenterPos, TextType.Damage);
            if (Model.IsDead)
            {
                OnDeathEvent();
            }

            await View.PlayDamageEft();
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

        public void AddStatusEffect(StatusEffect statEft)
        {
            statEft.SetView(View.CreateStatusEffectView());
            StatusEffects.Add(statEft);
            statEft.Init(this);
        }

        public virtual async UniTask ExecuteAction(Character target)
        {
            if (Model.IsDead) return;
            await UniTask.Yield();
        }

        public virtual async UniTask PrepareAction()
        {
            Model.Stats.Defence = 0;
            View.SetDefence(Model.Stats.Defence);
            foreach (var statEft in StatusEffects)
            {
                await statEft.Activate(this);
                if (statEft.Model.Turn <= 0)
                {
                    _expiredEffect.Add(statEft);
                }
            }
        }

        public virtual void EndAction()
        {
            foreach (var expired in _expiredEffect)
            {
                expired.Dispose(this);
                StatusEffects.Remove(expired);
            }
            _expiredEffect.Clear();
        }
        
        public async UniTask UseAp(float value)
        {
            Model.UseAp(value);
            await UniTask.Yield();
        }

        public void UseAp()
        {
            Model.UseAp();
        }

        public void HpRecover(float value)
        {
            Model.HpRecover(value);
            GameManager.Instance.CreateFloatingText(((int)value).ToString(), CenterPos, TextType.Heal);
            View.HpRecover(Model.CurHp, Model.MaxHp);
        }

        public void MaxHpUp(int value)
        {
            Model.MaxHpUp(value);
            View.HpRecover(Model.CurHp, Model.MaxHp);
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

        public void AddObserver(ICharacterObserver observer)
        {
            View.Observers.Add(observer);
        }

        public void RemoveObserver(ICharacterObserver observer)
        {
            View.Observers.Remove(observer);
        }

        public virtual void OnDeathEvent()
        {
            OnDeath?.Invoke();
        }

        public virtual void ResetAction()
        {
            OnDeath = null;
        }

        public void AddBuff(string statName, float value)
        {
            Model.AddBuff(statName, value);
        }

        public void RemoveBuff(string statName, float value)
        {
            Model.RemoveBuff(statName, value);
        }
        
        public async UniTask AddDefence(int value)
        {
            Model.AddDefence(value);
            View.SetDefence(Model.Defence);
            await UniTask.Yield();
        }

        public async UniTask PlayEffect(ParticleSystem activeEft)
        {
            await View.PlayEffect(activeEft);
        }
    }

    public class Enemy : Character
    {
        public EnemyModel eModel;

        [JsonIgnore]
        public EnemyView eView => View as EnemyView;
        
        public Enemy(EnemyModel model) : base(model)
        {
            eModel = model;
        }

        public override void Init()
        {
            base.Init();
            
        }
        public override void SetView(CharacterView view)
        {
            base.SetView(view);
            var task = SetAction();
        }

        public async UniTask SetAction()
        {
            if(IsExecutable())
                eModel.SetAction();
            await eView.SetActionView(eModel.GetCurAction());
        }

        public override async UniTask TakeDamage(float damage)
        {
            await base.TakeDamage(damage);
            if(Model.IsDead == false)
                await GameManager.Instance.user.ActivateArtifacts(ArtifactTrigger.EnemyDamaged, this);
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
                if (FindTag(StatTag.Stun, out _) == false)
                    curAct.Turn--;
                await eView.SetActionView(curAct);
                await eView.Wait();
            }

            if (Model.IsDead)
            {
                OnDeathEvent();
            }
        }

        public bool IsExecutable()
        {
            return eModel.GetCurAction().Turn <= 1 && FindTag(StatTag.Stun, out _) == false;
        }
    }

    public class Ally : Character
    {
        public AllyModel aModel;

        [JsonIgnore]
        public AllyView aView => View as AllyView;

        public Ally(AllyModel model) : base(model)
        {
            aModel = model;
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
                if(FindTag(StatTag.Stun, out _) == false)
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
            return aModel.GetCurAction().Turn <= 1 && FindTag(StatTag.Stun, out _) == false;
        }
    }

    public class Hero : Character
    {
        public HeroModel hModel;
        [JsonIgnore]
        public HeroView hView => View as HeroView;
        public Hero(HeroModel model) : base(model)
        {
            hModel = model;
        }


        public void ResetStat()
        {
            Model.Stats.Defence = 0;
            View.SetDefence(Model.Stats.Defence);
            ResetAction();
            foreach (var stat in StatusEffects)
            {
                stat.Dispose(this);
            }
            StatusEffects.Clear();
        }
    }
}
