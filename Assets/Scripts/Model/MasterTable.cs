using System.Collections.Generic;

namespace Model
{
    public class MasterTable
    {
        public List<MasterStage> MasterStages;
        public List<MasterEntity> MasterHeroes;
        public List<MasterEntity> MasterEnemies;
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
