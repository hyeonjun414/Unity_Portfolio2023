using System.Collections.Generic;

namespace Model
{
    public class MapNodeModel
    {
        public StageInfo StageData;
        public List<MapNodeModel> NextNodes;
        public int LinkCount;
        public int Step;

        public MapNodeModel(int step)
        {
            NextNodes = new List<MapNodeModel>();
            LinkCount = 0;
            Step = step;
        }

        public void AddNextStage(MapNodeModel node)
        {
            node.LinkCount++;
            NextNodes.Add(node);
        }
    }
}
