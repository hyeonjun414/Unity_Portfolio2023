using System.Collections.Generic;
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
        public float MaxActionGauge;
        public float CurActionGauge;
        public float ActionSpeed;
        public int ActionCount;
        public bool IsActionReady;
        public bool IsDead;

        public List<StatusEffectModel> StatusEffects;

        public EntityModel(MasterEntity me)
        {
            Id = me.Id;
            Name = me.Name;
            Desc = me.Desc;
            MaxHp = CurHp = me.Hp;
            Damage = me.Damage;
            MaxActionGauge = me.MaxActionGauge;
            CurActionGauge = 0;
            ActionSpeed = me.ActionSpeed;
            ActionCount = 0;
        }

        public void AddActionGauge(out bool isActionUp)
        {
            CurActionGauge += ActionSpeed * Time.deltaTime;
            if (CurActionGauge >= MaxActionGauge)
            {
                ActionCount++;
                CurActionGauge = 0;
                isActionUp = true;
                //IsActionReady = true;
            }

            isActionUp = false;
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

        public void AddStatusEffect(StatusEffectModel effectModel)
        {
            StatusEffects.Add(effectModel);
        }
    }
    public class EnemyModel : EntityModel
    {
        private List<EnemyAction> _actions;
        private EnemyAction _curAction;
        public EnemyModel(MasterEnemy me) : base(me)
        {
            _actions = Util.ToObjectList<EnemyAction>(me.Actions);
            if(_actions.Count != 0)
                SetAction();
        }

        public void SetAction()
        {
            _curAction = _actions[Random.Range(0, _actions.Count)];
            _curAction.Init();
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
