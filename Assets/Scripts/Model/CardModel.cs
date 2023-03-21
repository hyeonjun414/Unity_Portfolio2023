using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Presenter;
using UnityEngine;

namespace Model
{
    public class CardModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public string CardType;
        public string Effect;
        public CardFunction Function;

        public CardModel(MasterCard mc)
        {
            Id = mc.Id;
            Name = mc.Name;
            Desc = mc.Desc;
            CardType = mc.CardType;
            Effect = mc.Effect;
            string cardFunctionType = mc.Function["Type"].ToString();
            Function = JsonConvert.DeserializeObject<CfBomb>(mc.Function.ToString());
            
        }
    }

    public class CardFunction
    {
        public string Type;

        public virtual async UniTask Activate(EntityPresenter entity)
        {
        }

    }

    public class CfBomb : CardFunction
    {
        public float Damage;
        
        public override async UniTask Activate(EntityPresenter entity)
        {
            await entity.TakeDamage(Damage);
        }
    }
}
