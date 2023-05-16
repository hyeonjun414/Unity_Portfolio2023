using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Presenter;

namespace Model
{
    public class MasterTable
    {
        public List<MasterUser> MasterUsers;
        public List<MasterStage> MasterStages;
        public List<MasterHero> MasterHeroes;
        public List<MasterEnemy> MasterEnemies;
        public List<MasterAlly> MasterAllies;
        public List<MasterArtifact> MasterArtifacts;
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
        public int InitGold;
        public int DrawCardCount;
        public List<string> Cards;
        public List<string> Artifacts;
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
        public float Speed;
    }

    public class MasterEnemy : MasterEntity
    {
        public int DropGold;
        public List<JObject> Actions;
    }

    public class MasterHero : MasterEntity
    {

    }

    public class MasterAlly : MasterEntity
    {
        public List<JObject> Actions;
    }

    public class MasterCard
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Icon;
        public string Effect;
        public int Tier;
        public int Value;
        public int Cost;
        public CardType CardType;
        public List<JObject> Function;
    }

    public class MasterMap
    {
        public string Id;
        public string Name;
        public int Step;
        public int Width;
        public float MinLevelValue;
        public float MaxLevelValue;
    }

    public class MasterArtifact
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Icon;
        public int Tier;
        public int Value;
        public ArtifactTrigger Trigger;
        public List<JObject> Function;
    }

    
}
