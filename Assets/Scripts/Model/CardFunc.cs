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
            await UniTask.Yield();
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
    public class Cf_Damage : CardFunc
    {
        public float Damage;

        public override async UniTask Activate(Entity entity)
        {
            await entity.TakeDamage(Damage);
        }
    }

    public class Cf_ApDown : CardFunc
    {
        public float Value;
        public override async UniTask Activate(Entity entity)
        {
            await entity.UseAp(Value);

        }
    }

    public class Cf_HpRecover : CardFunc
    {
        public float Value;
        public override async UniTask Activate(Entity entity)
        {
            await entity.HpRecover(Value);
        }
    }
}
