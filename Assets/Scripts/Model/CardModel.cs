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
        public string Effect;
        public int Cost;
        public List<CardFunc> Function;

        public CardModel(MasterCard mc)
        {
            Id = mc.Id;
            Name = mc.Name;
            Desc = mc.Desc;
            Effect = mc.Effect;
            Cost = mc.Cost;
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
