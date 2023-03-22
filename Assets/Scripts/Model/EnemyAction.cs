using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public class EnemyAction
    {
        public string Type;
        public int Cost;
        public string Icon;

        public virtual async UniTask Activate(Enemy actor, Entity entity)
        {
        }

    }

    public class EaNormalAttack : EnemyAction
    {
        public float Damage;

        public override async UniTask Activate(Enemy actor, Entity entity)
        {
            await actor.PrepareAttack(entity.View.GetPosition());
            await actor.PlayAttack();
            await entity.TakeDamage(Damage);
            await actor.EndAttack();
        }
    }
}
