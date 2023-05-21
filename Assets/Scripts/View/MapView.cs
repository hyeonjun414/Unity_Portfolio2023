using System.Collections.Generic;
using Manager;
using Presenter;
using UnityEngine;

namespace View
{
    public class MapView : SceneView
    {
        public Map Presenter;

        public MapNodeView nodePrefab;
        public MapPathView pathPrefab;
        public List<List<MapNodeView>> StageNodeViews;
        public RectTransform nodePivot;
        public RectTransform pathPivot;
        
        public void GenerateStageNodes(List<List<MapNode>> stages)
        {
            // set contents size
            var totalHeight = stages.Count * 300f;
            nodePivot.sizeDelta = new Vector2(nodePivot.rect.width, totalHeight);
            nodePivot.localPosition = new Vector2(0, totalHeight / 2 + 50f);
            
            var stageNodes = new List<List<MapNodeView>>();
            
            
            for (var i = 0; i < stages.Count; i++)
            {
                var stageStep = new List<MapNodeView>();
                var totalWidth = stages[i].Count * 220;

                for (var j = 0; j < stages[i].Count; j++)
                {
                    var curMapNode = stages[i][j];

                    if (curMapNode == null)
                    {
                        stageStep.Add(null);
                    }
                    else
                    {
                        var node = Instantiate(nodePrefab, nodePivot);
                        node.transform.localPosition += new Vector3(
                            (-totalWidth / 2) + (j + 0.5f) * 220f,
                            (-totalHeight / 2) + i * 300f + 100f,
                            0);
                        node.Init(stages[i][j]);
                        stageStep.Add(node);
                    }
                }
                stageNodes.Add(stageStep);
            }

            StageNodeViews = stageNodes;
        }

        public void GeneratePath(MapNodeView start, MapNodeView end)
        {
            var path = Instantiate(pathPrefab, pathPivot);
            path.SetPath(start, end, canvasRect.localScale.x);
        }

        public void ActivateNextNodes(MapNodeView targetNode)
        {
            targetNode.ActiveNode(true);
        }
    }
}
