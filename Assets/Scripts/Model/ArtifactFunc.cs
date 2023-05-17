using Cysharp.Threading.Tasks;
using Manager;
using Presenter;
using UnityEngine;

namespace Model
{
    public class ArtifactFunc
    {
        public string Type;
        public int Value;

        public virtual void Init(User user)
        {
        }
        public virtual async UniTask Activate(Stage stage, User user)
        {
            await UniTask.Yield();
        }
    }

    public class AF_EnergyUp : ArtifactFunc
    {
        public override async UniTask Activate(Stage stage, User user)
        {
            await base.Activate(stage, user);
            if (stage is BattleStage bs)
            {
                await bs.AddEnergy(Value);
            }
        }
    }
    
    public class AF_MaxEnergyUp : ArtifactFunc
    {
        public override void Init(User user)
        {
            base.Init(user);
            user.AddMaxEnergy(Value);
        }
    }

    public class AF_MaxHpUp : ArtifactFunc
    {
        public override void Init(User user)
        {
            base.Init(user);
            user.UserHero.MaxHpUp(Value);
        }
    }

    public class AF_DefenceUp : ArtifactFunc
    {
        public override async UniTask Activate(Stage stage, User user)
        {
            await base.Activate(stage, user);
            await user.UserHero.AddDefence(Value);
        }
    }
}
