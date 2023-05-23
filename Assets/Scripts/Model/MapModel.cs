using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Model
{
    public class MapModel : SceneModel
    {
        public MapNodeModel StartNode;
        public MapNodeModel EndNode;
        public List<List<MapNodeModel>> MapNodes;

        public List<float> StageWeight;
        
        public float MinLevelValue;
        public float MaxLevelValue;

        public List<MasterStage> NormalStageList;
        public List<MasterStage> ShopStageList;
        public List<MasterStage> BossStageList;

        public void GenerateMap(MasterMap mm, MasterTable mt)
        {
            var step = mm.Step;
            var width = mm.Width;

            StageWeight = mm.StageWeight;
            
            MinLevelValue = mm.MinLevelValue;
            MaxLevelValue = mm.MaxLevelValue;

            NormalStageList = mt.MasterStages.Where(t => t.StageType == StageType.Normal).ToList();
            ShopStageList = mt.MasterStages.Where(t => t.StageType == StageType.Shop).ToList();
            BossStageList = mt.MasterStages.Where(t => t.StageType == StageType.Boss).ToList();
            

            StartNode = new MapNodeModel(-1, null, MinLevelValue, MaxLevelValue);
            EndNode = new MapNodeModel(step,
                BossStageList.Find(t => t.Id == mm.EndNodeId),MinLevelValue, MaxLevelValue);
            var mapList = new List<List<MapNodeModel>>();
            
            for (var i = 0; i < step; i++)
            {
                var stepStages = new List<MapNodeModel>();
                for (var j = 0; j < width; j++)
                {
                    stepStages.Add(null);
                }
                mapList.Add(stepStages);
            }
            MapNodes = mapList;
            ArrangeMap(mt);
        }

        public MasterStage PickRandomStage()
        {
            var randomValue = Random.value;
            var type = (StageType)Util.CalcChance(randomValue, StageWeight);
            switch (type)
            {
                case StageType.Normal:
                    return NormalStageList.OrderBy(t => randomValue).First();
                case StageType.Shop:
                    return ShopStageList.OrderBy(t => randomValue).First();
                case StageType.Boss:
                    return BossStageList.OrderBy(t => randomValue).First();
                default:
                    break;
            }
            return null;
        }

        public MasterStage PickStage(StageType type)
        {
            var randomValue = Random.value;
            switch (type)
            {
                case StageType.Normal:
                    return NormalStageList.OrderBy(t => randomValue).First();
                case StageType.Shop:
                    return ShopStageList.OrderBy(t => randomValue).First();
                case StageType.Boss:
                    return BossStageList.OrderBy(t => randomValue).First();
                default:
                    break;
            }

            return null;
        }

        public void ArrangeMap(MasterTable mt)
        {
            var firstStepNodes = MapNodes.First();
            for (var i = 0; i < firstStepNodes.Count; i++)
            {
                var randomStage = PickStage(StageType.Normal);
                firstStepNodes[i] = new MapNodeModel(0, randomStage,MinLevelValue, MaxLevelValue);
                StartNode.AddNextStage(firstStepNodes[i]);
            }

            for (var i = 0; i < MapNodes.Count-1; i++)
            { 
                for (var j = 0; j < MapNodes[i].Count; j++)
                {
                    var curNode = MapNodes[i][j];
                    if (curNode == null) 
                        continue;
                    for (var k = 0; k < curNode.LinkCount; k++)
                    {
                        var rand = Random.Range(-1, 2);
                        
                        var moveIndex = Math.Clamp(j + rand, 0, MapNodes[i + 1].Count - 1);

                        var randomStage = PickRandomStage();
                        if (MapNodes[i + 1][moveIndex] == null)
                        {
                            MapNodes[i + 1][moveIndex] = new MapNodeModel(i+1, randomStage,MinLevelValue, MaxLevelValue);
                        }
                        var nextNode = MapNodes[i + 1][moveIndex];
                        curNode.AddNextStage(nextNode);
                    }
                }
            }

            foreach (var node in MapNodes.Last())
            {
                node?.AddNextStage(EndNode);
            }
        }
    }
}
