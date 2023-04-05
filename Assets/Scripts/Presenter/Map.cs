using System;
using System.Collections.Generic;
using Model;
using Sirenix.Serialization;
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
                    Stage stageNode = null;
                    switch (stages[i][j].GetType())
                    {
                        case Type t when t == typeof(BattleStageModel):
                            stageNode = new BattleStage(stages[i][j], null);
                            break;
                        default:
                            stageNode = stageNode = new Stage(stages[i][j], null);
                            break;
                    }
                    stageStep.Add(stageNode);
                }
                stagePresenters.Add(stageStep);
            }

            Stages = stagePresenters;

            View.GenerateStageNodes(Stages);
        }
    }
}
