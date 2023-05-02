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

        public MapNodeModel(int step, float minLevelValue, float maxLevelValue)
        {
            NextNodes = new List<MapNodeModel>();
            LinkCount = 0;
            Step = step;
            MinLevelValue = minLevelValue;
            MaxLevelValue = maxLevelValue;
        }

        public void AddNextStage(MapNodeModel node)
        {
            node.LinkCount++;
            NextNodes.Add(node);
        }
    }
}
