using Cysharp.Threading.Tasks;
using Manager;
using Presenter;
using UnityEngine;

namespace Model
{
    public class ArtifactFunc
    {
        public string Type;

        public virtual async UniTask Init()
        {
            await UniTask.Yield();
        }
        public virtual async UniTask Activate()
        {
            await UniTask.Yield();
        }
    }

    public class Af_EnergyUp : ArtifactFunc
    {
        public int Value;
        public override async UniTask Activate()
        {
            await base.Activate();
            if (GameManager.Instance.CurStage is BattleStage bs)
            {
                await bs.AddEnergy(Value);
                
            }
        }
    }
    
    public class Af_MaxEnergyUp : ArtifactFunc
    {
        public int Value;

        public override async UniTask Init()
        {
            await base.Init();
            GameManager.Instance.User.AddMaxEnergy(Value);
        }
    }
}
