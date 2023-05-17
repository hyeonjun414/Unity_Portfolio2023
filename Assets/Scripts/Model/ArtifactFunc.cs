using Cysharp.Threading.Tasks;
using Manager;
using Presenter;
using UnityEngine;

namespace Model
{
    public class ArtifactFunc
    {
        public string Type;
        
        public virtual void Init(User user)
        {
        }
        public virtual async UniTask Activate(object target)
        {
            Debug.Log($"Active ArtifactFunc : {Type}");
            await UniTask.Yield();
        }
    }

    public class AF_EnergyUp : ArtifactFunc
    {
        public int Value;
        public override async UniTask Activate(object target)
        {
            await base.Activate(target);
            if (target is BattleStage bs)
            {
                await bs.AddEnergy(Value);
            }
        }
    }
    
    public class AF_MaxEnergyUp : ArtifactFunc
    {
        public int Value;
        public override void Init(User user)
        {
            base.Init(user);
            user.AddMaxEnergy(Value);
        }
    }

    public class AF_AllyAddBuff : ArtifactFunc
    {
        public string StatName;
        public int Value;
        public override async UniTask Activate(object target)
        {
            await base.Activate(target);
            if (target is Ally ally)
            {
                ally.AddBuff(StatName, Value);
            }
        }
    }

    public class AF_MaxHpUp : ArtifactFunc
    {
        public int Value;
        public override void Init(User user)
        {
            base.Init(user);
            user.UserHero.MaxHpUp(Value);
        }
    }

    public class AF_DefenceUp : ArtifactFunc
    {
        public int Value;
        public override async UniTask Activate(object target)
        {
            await base.Activate(target);
            if (target is User user)
            {
                await user.UserHero.AddDefence(Value);
            }
        }
    }
}
