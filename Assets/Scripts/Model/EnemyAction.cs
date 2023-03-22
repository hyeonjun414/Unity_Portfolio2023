using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public class EnemyAction
    {
        public string Type;
        public int Cost;

        public virtual async UniTask Activate(Entity entity)
        {
        }

    }

    public class EaNormalAttack : EnemyAction
    {
        public float Damage;

        public override async UniTask Activate(Entity entity)
        {
            await entity.TakeDamage(Damage);
        }
    }
}
