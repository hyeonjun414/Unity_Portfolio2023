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
        public TargetType TargetType;

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
}
