using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public class CardModel
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
        public List<CardFunc> Function;

        public CardModel()
        {
        }

        public CardModel(MasterCard mc)
        {
            Id = mc.Id;
            Name = mc.Name;
            Desc = mc.Desc;
            Icon = mc.Icon;
            Tier = mc.Tier;
            Value = mc.Value;
            Cost = mc.Cost;
            CardType = mc.CardType;
            Function = Util.ToObjectList<CardFunc>(mc.Function);
        }

        public async UniTask CardActivate(Character hero, Character character)
        {
            foreach (var func in Function)
            {
                await func.Activate(hero, character);
            }
        }
    }
}
