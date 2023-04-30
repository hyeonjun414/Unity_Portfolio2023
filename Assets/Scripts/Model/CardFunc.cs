using Cysharp.Threading.Tasks;
using Manager;
using Newtonsoft.Json.Linq;
using Presenter;

namespace Model
{
    public enum TextType
    {
        Damage,
        Heal,
    }
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
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.CreateFloatingText(Value.ToString(), entity.View.transform.position, TextType.Heal);
            await entity.HpRecover(Value);
        }
    }
}
