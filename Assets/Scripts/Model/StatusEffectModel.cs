using Cysharp.Threading.Tasks;
using Presenter;
using UnityEngine;
using View;

namespace Model
{
    public class StatusEffectModel
    {
        public int Turn;
        public string Icon;
        public float Value = 1;
        public StatTag StatTag = StatTag.None;
        public string Particle;
        public virtual void Init(Character character)
        {
            if(StatTag != StatTag.None)
                character.AddTag(StatTag, Value);
        }

        public virtual void Dispose(Character character)
        {
            if (StatTag != StatTag.None)
                character.RemoveTag(StatTag, Value);
        }
        
        public virtual async UniTask Activate(Character character)
        {
            Turn--;
            Debug.Log($"{character.Model.Name} : RemainTurn = {Turn}");

            await UniTask.Yield();
        }
    }

    public class SE_Burn : StatusEffectModel
    {
        public override async UniTask Activate(Character character)
        {
            await base.Activate(character);
            await character.TakeDamage(Value);
        }
    }

    public class SE_Recovery : StatusEffectModel
    {
        public override async UniTask Activate(Character character)
        {
            await base.Activate(character);
            await character.HpRecover(Value);
        }
    }

    public class SE_StatBuff : StatusEffectModel
    {
        public string StatName;
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
    }

    public class SE_Reflect : StatusEffectModel
    {
    }

    public class SE_Weak : StatusEffectModel
    {
    }

    public class SE_Stun : StatusEffectModel
    {
        
    }
     
}
