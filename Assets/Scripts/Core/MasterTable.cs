using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class MasterTable
    {
        public List<MasterStage> MasterStages;
        public List<MasterCharacter> MasterCharacters;
        public List<MasterEnemy> MasterEnemies;
    }
    public class MasterStage
    {
        public List<string> StageEnemies;
    }
    public class MasterCharacter
    {

    }
    
    public class MasterEnemy
    {
        public string Id;
        public string Name;
        public string Desc;
        public float Hp;
        public float Damage;
        public float MaxActionGauge;
        public float ActionSpeed;
    }

    
}
