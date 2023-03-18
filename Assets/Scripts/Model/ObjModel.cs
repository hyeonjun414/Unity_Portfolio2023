using System.Collections.Generic;
using System.Linq;
using View;

namespace Model
{
    public class User
    {
        public EntityModel Hero;

    }

    public class Entity
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

        public void Init(MasterEntity me)
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
    }
    public class Enemy : Entity
    {
    }
    public class Hero : Entity
    {
    }
}
