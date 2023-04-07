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
        
        public virtual async UniTask Activate(Enemy actor, Entity entity)
        {
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

        public override async UniTask Activate(Enemy actor, Entity entity)
        {
            await actor.PrepareAttack(entity.View.GetPosition());
            await actor.PlayAttack();
            await entity.TakeDamage(ActionValue);
            await actor.EndAttack();
        }
    }
}
