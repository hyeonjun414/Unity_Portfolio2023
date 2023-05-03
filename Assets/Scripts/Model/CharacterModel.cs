using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Model
{
    public class CharacterModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public float MaxHp;
        public float CurHp;
        public float Damage;
        public float Speed;
        public float MaxAp;
        public float CurAp;
        public bool IsReady;
        public bool IsDead;

        public List<StatusEffectModel> StatusEffects = new();
        private ReactiveProperty<float> _aprate = new ReactiveProperty<float>();
        public IReadOnlyReactiveProperty<float> ApRate => _aprate;

        public CharacterModel(MasterEntity me)
        {
            Id = me.Id;
            Name = me.Name;
            Desc = me.Desc;
            MaxHp = CurHp = me.Hp;
            Damage = me.Damage;
            Speed = me.Speed;
            CurAp = 0;
            MaxAp = 100;
        }
        
        public void TakeDamage(float damage)
        {
            CurHp -= damage;
            if (CurHp <= 0)
            {
                CurHp = 0;
                IsDead = true;
            }
        }


        public void AddAp(float deltaTime)
        {
            CurAp += Speed * deltaTime;
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
            CurHp = Mathf.Min(CurHp + value, MaxHp);
        }
    }
    public class EnemyModel : CharacterModel
    {
        private List<JObject> _actions;
        private CharacterAction _curAction;
        public EnemyModel(MasterEnemy me, float levelValue) : base(me)
        {
            _actions = me.Actions;

            Damage = Mathf.Round(Damage * levelValue);
            MaxHp = CurHp = Mathf.Round(CurHp * levelValue);
            
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
        }
    }
}
