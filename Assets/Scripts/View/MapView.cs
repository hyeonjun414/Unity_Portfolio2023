using System.Collections.Generic;
using Manager;
using Presenter;
using UnityEngine;

namespace View
{
    public class MapView : MonoBehaviour
    {
        public Map Presenter;

        public StageNodeView nodePrefab;
        public List<List<StageNodeView>> StageNodeViews;
        public RectTransform nodePivot;
        public void Start()
        {
            if (GameManager.Instance == null)
                return;
            
            Presenter = GameManager.Instance.CurMap;
            Presenter.View = this;
            Presenter.Init();
        }

        public void GenerateStageNodes(List<List<Stage>> stages)
        {
            // set contents size
            var totalHeight = stages.Count * 200f;
            nodePivot.sizeDelta = new Vector2(nodePivot.rect.width, totalHeight);
            nodePivot.position = new Vector2(0, totalHeight / 2);
            
            var stageNodes = new List<List<StageNodeView>>();
            
            
            for (var i = 0; i < stages.Count; i++)
            {
                var stageStep = new List<StageNodeView>();
                // Calculate the total width of this row of nodes
                var totalWidth = stages[i].Count * 150f;

                for (var j = 0; j < stages[i].Count; j++)
                {
                    var node = Instantiate(nodePrefab, nodePivot);
                    node.transform.localPosition += new Vector3(
                        (-totalWidth / 2) + (j + 0.5f) * 150f,
                        (-totalHeight / 2) + i * 200f + 100f,
                        0);
                    node.Init(stages[i][j]);
                    stageStep.Add(node);
                }

                stageNodes.Add(stageStep);
            }

            StageNodeViews = stageNodes;
        }
    }
}
