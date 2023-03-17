using System.Collections.Generic;
using System.Linq;
using Title;
using UnityEngine;

namespace Core
{
    public class Enemy
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
        public void Init(MasterEnemy me)
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
    public class Character
    {
    
    }

    public class Stage
    {
        public List<Enemy> Enemies = new();
        public void Init(MasterStage ms)
        {
            var masterTable = GameManager.Instance.MasterTable;

            foreach (var enemyId in ms.StageEnemies)
            {
                var masterEnemy = masterTable.MasterEnemies.First(target => target.Id == enemyId);
                var enemy = new Enemy();
                enemy.Init(masterEnemy);
                Enemies.Add(enemy);
            }
        }
    }
}
