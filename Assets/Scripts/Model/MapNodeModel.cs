using System.Collections.Generic;
using Manager;
using Presenter;

namespace Model
{
    public class MapNodeModel
    {
        public StageInfo StageData;
        public List<MapNodeModel> NextNodes;
        public StageModel stageModel;
        public int LinkCount;
        public int Step;

        public float MinLevelValue;
        public float MaxLevelValue;

        public MapNodeModel(int step, MasterStage randomStage, float minLevelValue, float maxLevelValue)
        {
            NextNodes = new List<MapNodeModel>();
            LinkCount = 0;
            Step = step;
            MinLevelValue = minLevelValue;
            MaxLevelValue = maxLevelValue;
            if(randomStage != null)
                StageData = Util.ToObject<StageInfo>(randomStage.StageInfo);
        }

        public void StageInit(MasterTable mt)
        {
            var user = GameManager.Instance.user;
            switch (StageData)
            {
                case BattleStageInfo:
                    stageModel = new BattleStageModel(this, mt);
                    break;
                case BossStageInfo:
                    stageModel = new BossStageModel(this, mt);
                    break;
                case ShopStageInfo:
                    stageModel = new ShopStageModel(this, user, mt);
                    break;
                case ChestStageInfo:
                    stageModel = new ChestStageModel(this, user, mt);
                    break;
            }
        }

        public void AddNextStage(MapNodeModel node)
        {
            node.LinkCount++;
            NextNodes.Add(node);
        }
    }
}
