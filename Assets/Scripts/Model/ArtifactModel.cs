using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Model
{
    public enum ArtifactTrigger
    {
        TurnStarted,
        EnemyDamaged,
        AllyDamaged,
        BattleStarted,
    }
    
    public class ArtifactModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Icon;
        public int Tier;
        public int Value;
        
        public ArtifactTrigger Trigger;
        public List<ArtifactFunc> Functions;

        public ArtifactModel(MasterArtifact ma)
        {
            Id = ma.Id;
            Name = ma.Name;
            Desc = ma.Desc;
            Tier = ma.Tier;
            Value = ma.Value;
            
            Icon = ma.Icon;
            Trigger = ma.Trigger;
            Functions = Util.ToObjectList<ArtifactFunc>(ma.Function);
        }

        public virtual async UniTask Activate(ArtifactTrigger trigger)
        {
            if (Trigger == trigger)
            {
                foreach (var func in Functions)
                {
                    await func.Activate();
                }
            }
            await UniTask.Yield();
        }
    }
}
