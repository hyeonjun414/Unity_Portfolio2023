using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Sirenix.Serialization;
using View;

namespace Presenter
{
    public class Map
    {
        public MapModel Model;
        public MapView View;

        public MapNode StartNode;
        public MapNode EndNode;
        public List<List<MapNode>> MapNodes;

        public MapNode CurNode;
        public Map(MapModel model, MapView view)
        {
            Model = model;
            View = view;
        }

        public void Init()
        {
            var stages = Model.MapNodes;
            var stagePresenters = new List<List<MapNode>>();
            StartNode = new MapNode(Model.StartNode, null);
            EndNode = new MapNode(Model.EndNode, null);

            CurNode = StartNode;
            for (var i = 0; i < stages.Count; i++)
            {
                var stageStep = new List<MapNode>();
                for (var j = 0; j < stages[i].Count; j++)
                {
                    var curMapNode = stages[i][j];

                    stageStep.Add(curMapNode == null ? null : new MapNode(curMapNode, null));
                }
                stagePresenters.Add(stageStep);
            }

            MapNodes = stagePresenters;

            View.GenerateStageNodes(MapNodes);

            for (var i = 0; i < MapNodes.Count-1; i++)
            {
                for (var j = 0; j < MapNodes[i].Count; j++)
                {
                    var curNode = MapNodes[i][j];
                    if (curNode == null) continue;
                    foreach (var nextNode in curNode.Model.NextNodes)
                    {
                        var targetNode = MapNodes[i + 1].FirstOrDefault(target => target != null && target.Model == nextNode);
                        if (targetNode == null) continue;
                        
                        View.GeneratePath(curNode.View, targetNode.View);
                    }
                }
            }

            foreach (var nextNode in CurNode.Model.NextNodes )
            {
                var targetNode = MapNodes[0].FirstOrDefault(target => target != null && target.Model == nextNode);
                if (targetNode == null) continue;
                
                View.ActivateNextNodes(targetNode.View);
            }
            
        }
    }
}
