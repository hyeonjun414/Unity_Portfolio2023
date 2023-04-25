using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;

namespace Model
{
    public class EntityModel
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

        public EntityModel(MasterEntity me)
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

        public void UseAp()
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
    }
    public class EnemyModel : EntityModel
    {
        private List<JObject> _actions;
        private EnemyAction _curAction;
        public EnemyModel(MasterEnemy me) : base(me)
        {
            _actions = me.Actions;
            if(_actions.Count != 0)
                SetAction();
        }

        public void SetAction()
        {
            _curAction = Util.ToObject<EnemyAction>(_actions[Random.Range(0, _actions.Count)]);
            _curAction.Init(this);
        }

        public EnemyAction GetCurAction()
        {
            return _curAction;
        }
    }

    public class HeroModel : EntityModel
    {
        public HeroModel(MasterEntity me) : base(me)
        {
        }
    }
}
