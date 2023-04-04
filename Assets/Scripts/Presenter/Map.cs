using System.Collections.Generic;
using Model;
using View;

namespace Presenter
{
    public class Map
    {
        public MapModel Model;
        public MapView View;


        public List<List<Stage>> Stages;
        public Map(MapModel model, MapView view)
        {
            Model = model;
            View = view;
        }

        public void Init()
        {
            var stages = Model.Maps;
            var stagePresenters = new List<List<Stage>>();
            for (var i = 0; i < stages.Count; i++)
            {
                var stageStep = new List<Stage>();
                for (var j = 0; j < stages[i].Count; j++)
                {
                    var stageNode = new Stage(stages[i][j], null);
                    stageStep.Add(stageNode);
                }
                stagePresenters.Add(stageStep);
            }

            Stages = stagePresenters;

            View.GenerateStageNodes(Stages);
        }
    }
}
