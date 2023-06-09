using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Presenter;
using UnityEngine.UI;

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
        public string Id;
        public string Name;
        public StageType StageType;
        public JObject StageInfo;
        public bool Selectable;
    }

    public class MasterEntity
    {
        public string Id;
        public string Name;
        public string Desc;
        public float Hp;
        public float Damage;
        public float Speed;
        public bool Selectable;
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
        public string EndNodeId;
        public float MinLevelValue;
        public float MaxLevelValue;
        public List<float> StageWeight;
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
