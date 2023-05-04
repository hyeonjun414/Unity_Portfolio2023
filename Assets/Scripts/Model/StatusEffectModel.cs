using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public class StatusEffectModel
    {
        public int Turn;
        public string Icon;
        public int Value;

        public virtual void Init(Character character)
        {
        }

        public virtual void Dispose(Character character)
        {
            
        }
        
        public virtual async UniTask Activate(Character character)
        {
            Turn--;
            if (Turn <= 0)
            {
            }

            await UniTask.Yield();
        }

        public int GetTurn() => Turn;
        public string GetIconName() => Icon;
        public int GetValue() => Value;
    }

    public class SE_Burn : StatusEffectModel
    {
        public float Damage;
        public string Particle;

        public override async UniTask Activate(Character character)
        {
            await base.Activate(character);
            await character.TakeDamage(Damage);
        }
    }

    public class SE_StatBuff : StatusEffectModel
    {
        public string StatName;
        public float Value;

        public override void Init(Character character)
        {
            base.Init(character);
            character.AddBuff(StatName, Value);
        }

        public override void Dispose(Character character)
        {
            base.Dispose(character);
            character.RemoveBuff(StatName, Value);
        }

        public override async UniTask Activate(Character character)
        {
            await base.Activate(character);
        }
    }
     
}
