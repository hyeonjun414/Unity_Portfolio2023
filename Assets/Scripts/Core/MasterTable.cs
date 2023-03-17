using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class MasterTable
    {
        public List<MasterStage> MasterStages;
        public List<MasterHero> MasterHeroes;
        public List<MasterEnemy> MasterEnemies;
    }
    public class MasterStage
    {
        public List<string> StageEnemies;
    }

    public class MasterEntity
    {
        public string Id;
        public string Name;
        public string Desc;
        public float Hp;
        public float Damage;
        public float MaxActionGauge;
        public float ActionSpeed;
    }

    public class MasterHero : MasterEntity
    {
    }

    public class MasterEnemy : MasterEntity
    {
    }

    
}
