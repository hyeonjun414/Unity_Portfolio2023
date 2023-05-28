using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Presenter;

namespace Model
{
    public class MapModel : SceneModel
    {
        public MapNodeModel StartNode;
        public MapNodeModel EndNode;
        public List<List<MapNodeModel>> MapNodes;

        public List<float> StageWeight;

        public int Step, Width;
        public float MinLevelValue;
        public float MaxLevelValue;

        public List<MasterStage> NormalStageList;
        public List<MasterStage> ShopStageList;
        public List<MasterStage> BossStageList;
        public List<MasterStage> ChestStageList;
        
        public void GenerateMap(MasterMap mm, MasterTable mt)
        {
            Step = mm.Step;
            Width = mm.Width;

            StageWeight = mm.StageWeight;
            
            MinLevelValue = mm.MinLevelValue;
            MaxLevelValue = mm.MaxLevelValue;

            NormalStageList = mt.MasterStages.Where(t => t.StageType == StageType.Normal).ToList();
            ShopStageList = mt.MasterStages.Where(t => t.StageType == StageType.Shop).ToList();
            BossStageList = mt.MasterStages.Where(t => t.StageType == StageType.Boss).ToList();
            ChestStageList = mt.MasterStages.Where(t => t.StageType == StageType.Chest).ToList();
            

            StartNode = new MapNodeModel(-1, null, MinLevelValue, MaxLevelValue);
            EndNode = new MapNodeModel(Step,
                BossStageList.Find(t => t.Id == mm.EndNodeId),MinLevelValue, MaxLevelValue);
            var mapList = new List<List<MapNodeModel>>();
            EndNode.StageInit(mt);
            
            for (var i = 0; i < Step; i++)
            {
                var stepStages = new List<MapNodeModel>();
                for (var j = 0; j < Width; j++)
                {
                    stepStages.Add(null);
                }
                mapList.Add(stepStages);
            }
            MapNodes = mapList;
            ArrangeMap(mt);
        }

        public MasterStage PickRandomStage(int step)
        {
            var randomValue = GameManager.Instance.Rand.Value;
            if (Step / 2 - 1 == step)
            {
                return ChestStageList.OrderBy(t => randomValue).First();
            }
            else
            {
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
            }
            
            return null;
        }

        public MasterStage PickStage(StageType type)
        {
            var randomValue = GameManager.Instance.Rand.Value;
            switch (type)
            {
                case StageType.Normal:
                    return NormalStageList.OrderBy(t => randomValue).First();
                case StageType.Shop:
                    return ShopStageList.OrderBy(t => randomValue).First();
                case StageType.Boss:
                    return BossStageList.OrderBy(t => randomValue).First();
                case StageType.Chest:
                    return ChestStageList.OrderBy(t => randomValue).First();
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
                firstStepNodes[i].StageInit(mt);
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
                        var rand = GameManager.Instance.Rand.Range(-1, 2);
                        
                        var moveIndex = Math.Clamp(j + rand, 0, MapNodes[i + 1].Count - 1);
                        
                        if (MapNodes[i + 1][moveIndex] == null)
                        {
                            var randomStage = PickRandomStage(i);
                            MapNodes[i + 1][moveIndex] = new MapNodeModel(i+1, randomStage,MinLevelValue, MaxLevelValue);
                            MapNodes[i + 1][moveIndex].StageInit(mt);
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
