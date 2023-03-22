using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public class CardFunc
    {
        public string Type;

        public virtual async UniTask Activate(Entity entity)
        {
        }

    }

    public class CfBomb : CardFunc
    {
        public float Damage;

        public override async UniTask Activate(Entity entity)
        {
            await entity.TakeDamage(Damage);
        }
    }
}
