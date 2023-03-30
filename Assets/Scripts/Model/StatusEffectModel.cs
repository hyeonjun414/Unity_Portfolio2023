using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public class StatusEffectModel
    {
        public int Turn;
        public string Icon;
        public int Value;

        public virtual void Init()
        {
            
        }
        
        public virtual async UniTask Activate(Entity entity)
        {
        }

        public int GetTurn() => Turn;
        public string GetIconName() => Icon;
        public int GetValue() => Value;
    }

    public class SE_Burn : StatusEffectModel
    {
        public float Damage;
        public string Particle;

        public override async UniTask Activate(Entity entity)
        {
            await entity.TakeDamage(Damage);
        }
    }
     
}
