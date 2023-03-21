using Newtonsoft.Json;
using UnityEngine;

namespace Model
{
    public class CardModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public string CardType;
        public CardFunction Function;

        public CardModel(MasterCard mc)
        {
            Id = mc.Id;
            Name = mc.Name;
            Desc = mc.Desc;
            CardType = mc.CardType;
            Function = JsonConvert.DeserializeObject<CardFunction>(mc.Function.ToString());
        }
    }

    public class CardFunction
    {
        public string Type;

        public virtual void Activate()
        {
            
        }
    }

    public class CfBomb : CardFunction
    {
        public float Damage;
    }
}
