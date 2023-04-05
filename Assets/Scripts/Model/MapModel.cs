using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Model
{
    public class MapModel
    {
        public List<List<MapNodeModel>> MapNodes;

        public void GenerateMap(MasterMap mm, MasterTable mt)
        {
            var step = mm.Step;
            var width = mm.Width;

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
            
            for (var i = 0; i < MapNodes[0].Count; i++)
            {
                MapNodes[0][i] = new MapNodeModel();
                var randomStage = stageList.OrderBy(t => Random.value).First();
                MapNodes[0][i].StageData = Util.ToObject<StageInfo>(randomStage.StageInfo);
                MapNodes[0][i].LinkCount++;
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
                            MapNodes[i + 1][moveIndex] = new MapNodeModel();
                            MapNodes[i + 1][moveIndex].StageData = Util.ToObject<StageInfo>(randomStage.StageInfo);
                        }
                        var nextNode = MapNodes[i + 1][moveIndex];
                        curNode.AddNextStage(nextNode);
                    }
                }

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
