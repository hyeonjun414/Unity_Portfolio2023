using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Model
{
    public class MapModel
    {
        public List<List<StageModel>> Maps;

        public void GenerateMap(MasterMap mm, MasterTable mt)
        {
            var step = mm.Step;
            var width = mm.Width;

            var stageList = mt.MasterStages;

            var mapList = new List<List<StageModel>>();
            
            for (var i = 0; i < step; i++)
            {
                var stepStages = new List<StageModel>();
                for (var j = 0; j < width; j++)
                {
                    var randomStage = stageList.OrderBy(t => Random.value).First();
                    var stage = new StageModel(Util.ToObject<StageInfo>(randomStage.StageInfo), mt);
                    stepStages.Add(stage);
                }
                mapList.Add(stepStages);
            }

            Maps = mapList;
        }

        public int GetStep() => Maps.Count;
        public int GetStepWidth(int step) => Maps[step].Count;
    }
}
