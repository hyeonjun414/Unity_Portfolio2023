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
        public int Cost;
        public CardType CardType;
        public List<CardFunc> Function;

        public CardModel(MasterCard mc)
        {
            Id = mc.Id;
            Name = mc.Name;
            Desc = mc.Desc;
            Icon = mc.Icon;
            Effect = mc.Effect;
            Cost = mc.Cost;
            CardType = mc.CardType;
            Function = Util.ToObjectList<CardFunc>(mc.Function);
        }

        public async UniTask CardActivate(Enemy enemy)
        {
            foreach (var func in Function)
            {
                await func.Activate(enemy);
            }
        }
    }
}
