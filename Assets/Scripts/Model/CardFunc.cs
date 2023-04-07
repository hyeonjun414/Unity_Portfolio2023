using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Presenter;

namespace Model
{
    public class CardFunc
    {
        public string Type;

        public virtual async UniTask Activate(Entity entity)
        {
        }

    }

    public class CfStatusEffect : CardFunc
    {
        public JObject StatusEffect;

        public override async UniTask Activate(Entity entity)
        {
            var effect = Util.ToObject<StatusEffectModel>(StatusEffect);
            
            await entity.AddStatusEffect(effect);
        }
    }
    public class CfBomb : CardFunc
    {
        public float Damage;

        public override async UniTask Activate(Entity entity)
        {
            await entity.TakeDamage(Damage);
        }
    }

    public class CfDefence : CardFunc
    {
        // public override async UniTask Activate(Entity entity)
        // {
        //     return 
        // }
    }
}
