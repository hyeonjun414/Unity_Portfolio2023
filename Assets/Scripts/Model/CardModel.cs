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
        public CardFunction Function;

        public CardModel(MasterCard mc)
        {
            Id = mc.Id;
            Name = mc.Name;
            Desc = mc.Desc;
            CardType = mc.CardType;
            Effect = mc.Effect;
            Function = Util.ToObject<CardFunction>(mc.Function);
        }
    }

    public class CardFunction
    {
        public string Type;

        public virtual async UniTask Activate(Entity entity)
        {
        }

    }

    public class CfBomb : CardFunction
    {
        public float Damage;
        
        public override async UniTask Activate(Entity entity)
        {
            await entity.TakeDamage(Damage);
        }
    }
}
