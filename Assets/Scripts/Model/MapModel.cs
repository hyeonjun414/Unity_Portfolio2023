using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Model
{
    public class MapModel
    {
        public MapNodeModel StartNode;
        public MapNodeModel EndNode;
        public List<List<MapNodeModel>> MapNodes;

        public void GenerateMap(MasterMap mm, MasterTable mt)
        {
            var step = mm.Step;
            var width = mm.Width;

            StartNode = new MapNodeModel(-1);
            EndNode = new MapNodeModel(step);
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

        public void ArrangeMap(MasterTable mt)
        {
            var stageList = mt.MasterStages;

            var firstStepNodes = MapNodes.First();
            for (var i = 0; i < firstStepNodes.Count; i++)
            {
                firstStepNodes[i] = new MapNodeModel(0);
                var randomStage = stageList.OrderBy(t => Random.value).First();
                firstStepNodes[i].StageData = Util.ToObject<StageInfo>(randomStage.StageInfo);
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
                        
                        var randomStage = stageList.OrderBy(t => Random.value).First();
                        if (MapNodes[i + 1][moveIndex] == null)
                        {
                            MapNodes[i + 1][moveIndex] = new MapNodeModel(i+1);
                            MapNodes[i + 1][moveIndex].StageData = Util.ToObject<StageInfo>(randomStage.StageInfo);
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

        public StageModel GenerateStage(MasterStage ms, MasterTable mt)
        {
            var stageInfo = Util.ToObject<StageInfo>(ms.StageInfo);
            StageModel genStage = null;
            switch (stageInfo.Type)
            {
                case nameof(BattleStageInfo):
                    genStage = new BattleStageModel(stageInfo, mt);
                    break;
            }

            return genStage;
        }

        public int GetStep() => MapNodes.Count;
        public int GetStepWidth(int step) => MapNodes[step].Count;
    }
}