using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Presenter;

namespace Model
{
    public enum ArtifactTrigger
    {
        None,
        TurnStarted,
        TurnEnded,
        EnemyDamaged,
        AllyDamaged,
        BattleStarted,
        AllySummoned,
    }
    
    public class ArtifactModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public string Icon;
        public int Tier;
        public int Value;
        public bool IsActive;
        
        public ArtifactTrigger Trigger;
        public List<ArtifactFunc> Functions;

        public ArtifactModel()
        {
        }
        
        public ArtifactModel(MasterArtifact ma)
        {
            Id = ma.Id;
            Name = ma.Name;
            Desc = ma.Desc;
            Tier = ma.Tier;
            Value = ma.Value;
            
            Icon = ma.Icon;
            Trigger = ma.Trigger;
            IsActive = false;
            Functions = Util.ToObjectList<ArtifactFunc>(ma.Function);
        }

        public virtual void Init(User user)
        {
            if (user == null) return;
            
            foreach (var func in Functions)
            {
                func.Init(user);
            }
        }
        
        public virtual async UniTask Activate(ArtifactTrigger trigger, object target)
        {
            if (Trigger == trigger)
            {
                foreach (var func in Functions)
                {
                    if (func.ConditionCheck(target) && !IsActive)
                    {
                        IsActive = true;
                        await func.Activate(target);
                    }
                        
                }
                
            }
            await UniTask.Yield();
        }
    }
}
