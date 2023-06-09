using System.Linq;
using Cysharp.Threading.Tasks;
using Manager;
using Newtonsoft.Json.Linq;
using Presenter;
using UnityEngine;
namespace Model
{
    public enum TargetType
    {
        Single,
        All,
        Spread,
        Random,
        Front,
        Back,
        Hero,
    }
    
    public class CardFunc
    {
        public string Type;
        public string Effect;
        public TargetType TargetType = TargetType.Single;
        
        public virtual async UniTask Activate(Character hero, Character target)
        {
            await UniTask.Yield();
        }

        public virtual void CreateEft(Transform pivot)
        {
            if (Effect != null)
                GameManager.Instance.CreateEft(Effect, pivot);
        }
    }

    public class Cf_StatusEffect : CardFunc
    {
        public JObject StatusEffect;

        public override async UniTask Activate(Character hero, Character target)
        {
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                var targetList = curStage.GetTarget(target, TargetType);
                foreach (var t in targetList)
                {
                    var effectModel = Util.ToObject<StatusEffectModel>(StatusEffect);
                    CreateEft(t.CenterPivot);
                    t.AddStatusEffect(new StatusEffect(effectModel));
                    await UniTask.Yield();
                }
            }
        }
    }
    public class Cf_Damage : CardFunc
    {
        public int Damage;

        public override async UniTask Activate(Character hero, Character target)
        {
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                var targetList = curStage.GetTarget(target, TargetType);
                foreach (var t in targetList)
                {
                    CreateEft(t.CenterPivot);
                    await hero.Attack(t, Damage);
                }
            }
        }
    }

    public class Cf_ApDown : CardFunc
    {
        public float Value;
        public override async UniTask Activate(Character hero, Character target)
        {
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                var targetList = curStage.GetTarget(target, TargetType);
                foreach (var t in targetList)
                {
                    CreateEft(t.CenterPivot);
                    await t.UseAp(Value);
                }
            }
        }
    }

    public class Cf_HpRecover : CardFunc
    {
        public float Value;
        public override async UniTask Activate(Character hero, Character target)
        {
            await base.Activate(hero, target);
            CreateEft(target.CenterPivot);
            target.HpRecover(Value);
        }
    }

    public class Cf_SummonAlly : CardFunc
    {
        public string Character;
        public int LivingTurn;

        public override async UniTask Activate(Character hero, Character target)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                CreateEft(hero.CenterPivot);
                await curStage.SummonAlly(Character, LivingTurn);
            }
        }
    }

    public class Cf_PositionSwitch : CardFunc
    {
        public int MoveIndex;

        public override async UniTask Activate(Character hero, Character target)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                CreateEft(target.CenterPivot);
                await curStage.PositionSwitch(target, MoveIndex);
            }
        }
    }

    public class Cf_DrawCard : CardFunc
    {
        public int DrawCount;

        public override async UniTask Activate(Character hero, Character target)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                CreateEft(hero.CenterPivot);
                await curStage.DrawCard(DrawCount);
            }
        }
    }

    public class Cf_DefenceUp : CardFunc
    {
        public int Value;

        public override async UniTask Activate(Character hero, Character target)
        {
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                var targetList = curStage.GetTarget(target, TargetType);
                foreach (var t in targetList)
                {
                    CreateEft(t.CenterPivot);
                    await t.AddDefence(Value);
                }
            }
        }
    }

    public class Cf_GetGold : CardFunc
    {
        public int Value;

        public override async UniTask Activate(Character hero, Character target)
        {
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                CreateEft(hero.CenterPivot);
                curStage.UserGetGold(Value);
            }
        }
    }
}
