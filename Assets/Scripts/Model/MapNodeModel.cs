using System.Collections.Generic;

namespace Model
{
    public class MapNodeModel
    {
        public StageInfo StageData;
        public List<MapNodeModel> NextNodes;
        public int LinkCount;

        public MapNodeModel()
        {
            NextNodes = new List<MapNodeModel>();
            LinkCount = 0;
        }

        public void AddNextStage(MapNodeModel node)
        {
            node.LinkCount++;
            NextNodes.Add(node);
        }
    }
}
