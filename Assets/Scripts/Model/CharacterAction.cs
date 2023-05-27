using Cysharp.Threading.Tasks;
using Manager;
using Presenter;

namespace Model
{
    public class CharacterAction
    {
        public string Type;
        public int Turn;
        public string Icon;
        public string ActionType;

        protected int ActionValue;
        public TargetType TargetType = TargetType.Single;

        public virtual void Init(CharacterModel charModel)
        {
        }
        
        public virtual async UniTask Activate(Character actor, Character target)
        {
            await UniTask.Yield();
        }

        public int GetValue()
        {
            return ActionValue;
        }


    }

    public class Ca_NormalAttack : CharacterAction
    {
        public float Damage;
        
        public override void Init(CharacterModel charModel)
        {
            base.Init(charModel);
            ActionValue = (int)(charModel.Damage * Damage);
        }

        public override async UniTask Activate(Character actor, Character target)
        {
            await actor.PrepareAttack(target.WorldPosition);
            await actor.PlayAttack();
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                foreach (var t in curStage.GetTarget(target, TargetType))
                {
                    await actor.Attack(t, ActionValue);
                }
            }
            await actor.EndAttack();
        }
    }

    public class Ca_Defence : CharacterAction
    {
        public float Value;

        public override void Init(CharacterModel charModel)
        {
            base.Init(charModel);
            ActionValue = (int)(charModel.Damage * Value);
        }

        public override async UniTask Activate(Character actor, Character target)
        {
            await actor.PlayAttack();
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                foreach (var t in curStage.GetTarget(actor, TargetType))
                {
                    await t.AddDefence(ActionValue);
                }
            }
            await actor.EndAttack();
        }
    }

    public class Ca_HpRecover : CharacterAction
    {
        public float Value;

        public override void Init(CharacterModel charModel)
        {
            base.Init(charModel);
            ActionValue = (int)(charModel.Damage * Value);
        }

        public override async UniTask Activate(Character actor, Character target)
        {
            await actor.PlayAttack();
            if (GameManager.Instance.CurStage is BattleStage curStage)
            {
                foreach (var t in curStage.GetTarget(actor, TargetType))
                {
                    t.HpRecover(ActionValue);
                }
            }
            await actor.EndAttack();
        }
    }
}
