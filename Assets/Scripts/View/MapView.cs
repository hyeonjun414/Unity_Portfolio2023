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
        public Transform nodePivot;
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
            var stageNodes = new List<List<StageNodeView>>();
            for (var i = 0; i < stages.Count; i++)
            {
                var stageStep = new List<StageNodeView>();
                for (var j = 0; j < stages[i].Count; j++)
                {
                    var node = Instantiate(nodePrefab, nodePivot);
                    node.transform.localPosition += new Vector3(j * 150, i * 200, 0);
                    node.Init(stages[i][j]);
                    stageStep.Add(node);
                }

                stageNodes.Add(stageStep);
            }

            StageNodeViews = stageNodes;
        }
    }
}
