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
        Miss,
    }
    public class CardFunc
    {
        public string Type;

        public virtual async UniTask Activate(Character character)
        {
            await UniTask.Yield();
        }

    }

    public class CfStatusEffect : CardFunc
    {
        public JObject StatusEffect;

        public override async UniTask Activate(Character character)
        {
            var effect = Util.ToObject<StatusEffectModel>(StatusEffect);
            
            await character.AddStatusEffect(effect);
        }
    }
    public class Cf_Damage : CardFunc
    {
        public float Damage;
        public bool IsAll;

        public override async UniTask Activate(Character character)
        {
            if (IsAll)
            {
                var curStage = GameManager.Instance.CurStage as BattleStage;
                if (curStage != null)
                {
                    var targetList = character is Enemy ? curStage.Enemies : curStage.Allies;
                    foreach (var target in targetList)
                    {
                        await target.TakeDamage(Damage);
                    }
                }
            }
            else
            {
                await character.TakeDamage(Damage); 
            }
            
        }
    }

    public class Cf_ApDown : CardFunc
    {
        public float Value;
        public override async UniTask Activate(Character character)
        {
            await character.UseAp(Value);

        }
    }

    public class Cf_HpRecover : CardFunc
    {
        public float Value;
        public override async UniTask Activate(Character character)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.CreateFloatingText(Value.ToString(), character.View.transform.position, TextType.Heal);
            await character.HpRecover(Value);
        }
    }

    public class Cf_SummonAlly : CardFunc
    {
        public string Character;
        public int LivingTurn;

        public override async UniTask Activate(Character character)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                await curStage.SummonAlly(Character, LivingTurn);
            }
        }
    }

    public class Cf_PositionSwitch : CardFunc
    {
        public int MoveIndex;

        public override async UniTask Activate(Character character)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                await curStage.PositionSwitch(character, MoveIndex);
            }
        }
    }

    public class Cf_DrawCard : CardFunc
    {
        public int DrawCount;

        public override async UniTask Activate(Character character)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                await curStage.DrawCard(DrawCount);
            }
        }
    }
}
