using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Presenter;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Model
{
    public struct CharacterStat
    {
        public float MaxHp;
        public float CurHp;
        public float Damage;
        public float Speed;
        public float HitRate;
        public float Defence;
    }
    public class CharacterModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public bool IsReady;
        public bool IsDead;
        public float MaxAp;
        public float CurAp;
        public CharacterType CharType;

        public float MaxHp => Stats.MaxHp + BuffStats.MaxHp;
        public float CurHp => Stats.CurHp + BuffStats.CurHp;
        public float Damage => Stats.Damage + BuffStats.Damage < 0 ? 0 : Stats.Damage + BuffStats.Damage;
        public float Speed => Stats.Speed + BuffStats.Speed < 0 ? 0 : Stats.Speed + BuffStats.Speed;
        public float HitRate => Stats.HitRate + BuffStats.HitRate < 0 ? 0 : Stats.HitRate + BuffStats.HitRate;
        public float Defence => Stats.Defence;
        
        public CharacterStat Stats = new();
        public CharacterStat BuffStats = new();

        public List<StatusEffectModel> StatusEffects = new();
        private ReactiveProperty<float> _aprate = new ReactiveProperty<float>();
        public IReadOnlyReactiveProperty<float> ApRate => _aprate;

        public CharacterModel(MasterEntity me)
        {
            Id = me.Id;
            Name = me.Name;
            Desc = me.Desc;
            Stats.MaxHp = me.Hp;
            Stats.CurHp = me.Hp;
            Stats.Damage = me.Damage;
            Stats.Speed = me.Speed;
            Stats.HitRate = 1;
            
            CurAp = 0;
            MaxAp = 100;
        }
        
        public void TakeDamage(float damage)
        {
            var prevDef = Stats.Defence;
            Stats.Defence = Mathf.Max(Stats.Defence - damage, 0);
            damage = Mathf.Max(damage - prevDef, 0);
            if (damage == 0) return;
            Stats.CurHp -= damage;
            if (Stats.CurHp <= 0)
            {
                Stats.CurHp = 0;
                IsDead = true;
            }
        }


        public void AddAp(float deltaTime)
        {
            CurAp += Stats.Speed * deltaTime;
            _aprate.Value = CurAp / MaxAp;
            if (CurAp >= MaxAp)
            {
                IsReady = true;
                CurAp = MaxAp;
            }
        }

        public virtual void UseAp()
        {
            CurAp = 0;
            IsReady = false;
            _aprate.Value = CurAp / MaxAp;
        }

        public void UseAp(float value)
        {
            CurAp = Mathf.Max(CurAp - value, 0);
            IsReady = false;
            _aprate.Value = CurAp / MaxAp;
        }

        public void HpRecover(float value)
        {
            Stats.CurHp = Mathf.Min(Stats.CurHp + value, Stats.MaxHp);
        }

        public void AddStatusEffect(StatusEffectModel statEft)
        {
            StatusEffects.Add(statEft);
        }

        public void AddBuff(string statName, float value)
        {
            switch (statName)
            {
                case "Damage":
                    BuffStats.Damage += value;
                    break;
                case "HitRate":
                    BuffStats.HitRate += value;
                    break;
            }
        }

        public void RemoveBuff(string statName, float value)
        {
            switch (statName)
            {
                case "Damage":
                    BuffStats.Damage -= value;
                    break;
                case "HitRate":
                    BuffStats.HitRate -= value;
                    break;
            }
        }

        public void AddDefence(int value)
        {
            Stats.Defence += value;
        }
    }
    public class EnemyModel : CharacterModel
    {
        public int DropGold;
        private List<JObject> _actions;
        private CharacterAction _curAction;
        public EnemyModel(MasterEnemy me, float levelValue) : base(me)
        {
            CharType = CharacterType.Enemy;
            _actions = me.Actions;
            DropGold = (int)(me.DropGold * levelValue);
            Stats.Damage = Mathf.Round(Stats.Damage * levelValue);
            Stats.MaxHp = Stats.CurHp = Mathf.Round(Stats.CurHp * levelValue);
            
            if(_actions.Count != 0)
                SetAction();
        }

        public void SetAction()
        {
            _curAction = Util.ToObject<CharacterAction>(_actions[Random.Range(0, _actions.Count)]);
            _curAction.Init(this);
        }

        public CharacterAction GetCurAction()
        {
            return _curAction;
        }
    }

    public class AllyModel : CharacterModel
    {
        private List<JObject> _actions;
        private CharacterAction _curAction;
        
        public int Turn;
        public AllyModel(MasterAlly ma, int livingTurn) : base(ma)
        {
            CharType = CharacterType.Ally;
            _actions = ma.Actions;
            Turn = livingTurn;

            if (_actions.Count != 0)
                SetAction();
        }

        public override void UseAp()
        {
            base.UseAp();
            Turn--;
            if (Turn <= 0)
            {
                IsDead = true;
            }
        }

        public void SetAction()
        {
            _curAction = Util.ToObject<CharacterAction>(_actions[Random.Range(0, _actions.Count)]);
            _curAction.Init(this);
        }

        public CharacterAction GetCurAction()
        {
            return _curAction;
        }
    }

    public class HeroModel : CharacterModel
    {
        public HeroModel(MasterEntity me) : base(me)
        {
            CharType = CharacterType.Ally;
        }
    }
}
