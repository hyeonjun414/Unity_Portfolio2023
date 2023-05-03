using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public class EnemyAction
    {
        public string Type;
        public int Turn;
        public string Icon;
        public string ActionType;

        protected int ActionValue;

        public virtual void Init(EnemyModel enemy)
        {
        }
        
        public virtual async UniTask Activate(Enemy actor, Character character)
        {
            await UniTask.Yield();
        }

        public int GetValue()
        {
            return ActionValue;
        }


    }

    public class EaNormalAttack : EnemyAction
    {
        public float Damage;

        public override void Init(EnemyModel enemy)
        {
            base.Init(enemy);
            ActionValue = (int)(enemy.Damage * Damage);
        }

        public override async UniTask Activate(Enemy actor, Character character)
        {
            await actor.PrepareAttack(character.View.GetPosition());
            await actor.PlayAttack();
            await character.TakeDamage(ActionValue);
            await actor.EndAttack();
        }
    }
}
