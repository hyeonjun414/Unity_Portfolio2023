using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Presenter;

namespace Model
{
    public class CardModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public string CardType;
        public string Effect;
        public CardFunc Function;

        public CardModel(MasterCard mc)
        {
            Id = mc.Id;
            Name = mc.Name;
            Desc = mc.Desc;
            CardType = mc.CardType;
            Effect = mc.Effect;
            Function = Util.ToObject<CardFunc>(mc.Function);
        }
    }
}
