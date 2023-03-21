using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Model
{
    public class MasterTable
    {
        public List<MasterUser> MasterUsers;
        public List<MasterStage> MasterStages;
        public List<MasterEntity> MasterHeroes;
        public List<MasterEntity> MasterEnemies;
        public List<MasterCard> MasterCards;
    }

    public class MasterUser
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Hero;
        public List<string> Cards;
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

    public class MasterCard
    {
        public string Id;
        public string Name;
        public string Desc;
        public string CardType;
        public JToken Function;
    }

    
}
