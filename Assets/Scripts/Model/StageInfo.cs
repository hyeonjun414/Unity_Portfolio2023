using System.Collections.Generic;

namespace Model
{
    public class StageInfo
    {
        public string Type;
        public string Icon;
    }

    public class BattleStageInfo : StageInfo
    {
        public int MinCount;
        public int MaxCount;
    }

    public class BossStageInfo : StageInfo
    {
        public string BossId;
    }
    
    public class ShopStageInfo : StageInfo
    {
        public int CardCount;
        public int ArtifactCount;
        public List<int> TierChance;
    }
}
