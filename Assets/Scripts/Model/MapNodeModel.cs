using System.Collections.Generic;

namespace Model
{
    public class MapNodeModel
    {
        public StageInfo StageData;
        public List<MapNodeModel> NextNodes;
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

        public void AddNextStage(MapNodeModel node)
        {
            node.LinkCount++;
            NextNodes.Add(node);
        }
    }
}
