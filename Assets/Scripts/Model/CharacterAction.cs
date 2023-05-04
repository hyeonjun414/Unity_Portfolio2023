using Cysharp.Threading.Tasks;
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

    public class EaNormalAttack : CharacterAction
    {
        public float Damage;

        public override void Init(CharacterModel charModel)
        {
            base.Init(charModel);
            ActionValue = (int)(charModel.Damage * Damage);
        }

        public override async UniTask Activate(Character actor, Character target)
        {
            await actor.PrepareAttack(target.View.GetPosition());
            await actor.PlayAttack();
            await actor.Attack(target, ActionValue);
            //await target.TakeDamage(ActionValue);
            await actor.EndAttack();
        }
    }
}
