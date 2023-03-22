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
        public bool IsActionReady;
        public bool IsDead;

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
        }

        public void AddActionGauge()
        {
            CurActionGauge += ActionSpeed * Time.deltaTime;
            if (CurActionGauge >= MaxActionGauge)
            {
                CurActionGauge = MaxActionGauge;
                IsActionReady = true;
            }
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
    }
    public class EnemyModel : EntityModel
    {
        public List<EnemyAction> Actions;
        public EnemyModel(MasterEnemy me) : base(me)
        {
            Actions = Util.ToObjectList<EnemyAction>(me.Actions);
        }
    }

    public class HeroModel : EntityModel
    {
        public HeroModel(MasterEntity me) : base(me)
        {
        }
    }
}
