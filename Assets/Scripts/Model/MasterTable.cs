using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Model
{
    public class MasterTable
    {
        public List<MasterUser> MasterUsers;
        public List<MasterStage> MasterStages;
        public List<MasterHero> MasterHeroes;
        public List<MasterEnemy> MasterEnemies;
        public List<MasterCard> MasterCards;
        public List<MasterMap> MasterMaps;
    }

    public class MasterUser
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Hero;
        public int Energy;
        public int DrawCardCount;
        public List<string> Cards;
    }
    public class MasterStage
    {
        public JObject StageInfo;
    }

    public class MasterEntity
    {
        public string Id;
        public string Name;
        public string Desc;
        public float Hp;
        public float Damage;
    }

    public class MasterEnemy : MasterEntity
    {
        public List<JObject> Actions;
    }

    public class MasterHero : MasterEntity
    {

    }

    public class MasterCard
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Icon;
        public string Effect;
        public int Cost;
        public List<JObject> Function;
    }

    public class MasterMap
    {
        public string Id;
        public string Name;
        public int Step;
        public int Width;
    }

    
}
