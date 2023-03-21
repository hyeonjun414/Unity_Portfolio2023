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
            Function = ToObject<CardFunction>(mc.Function);
        }

        public static T ToObject<T>(JObject data)
        {
            var type = typeof(CardFunction);
            var domainPrefix = type.FullName;
            domainPrefix = domainPrefix.Remove(domainPrefix.Length - type.Name.Length);
            if (data["Type"] != null)
            {
                type = Type.GetType(domainPrefix + data["Type"]);
            }
            return (T)JsonConvert.DeserializeObject(data.ToString(), type);
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
