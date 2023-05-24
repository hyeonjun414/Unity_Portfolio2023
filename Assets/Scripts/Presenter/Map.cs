using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using Newtonsoft.Json;
using View;

namespace Presenter
{
    public class Map : Scene
    {
        public MapModel mModel;
        
        [JsonIgnore]
        public MapView mView => View as MapView;
        public MapNode StartNode;
        public MapNode EndNode;
        public List<List<MapNode>> MapNodes;
        public MapNode CurNode;
        public int CurStep;

        public Map()
        {
        }
        
        public Map(GameManager gm, MapModel model, MasterMap mm, MasterTable mt) : base(gm, model)
        {
            mModel = model;
            Init(gm.GameCore.User, mm, mt);
        }

        public void Init(User user, MasterMap mm, MasterTable mt)
        {
            mModel.GenerateMap(user, mm, mt);
            var stages = mModel.MapNodes;
            var stagePresenters = new List<List<MapNode>>();
            StartNode = new MapNode(mModel.StartNode, null);
            EndNode = new MapNode(mModel.EndNode, null);

            CurNode = StartNode;
            CurStep = -1;
            stages.Add(new List<MapNodeModel>(){ mModel.EndNode });
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
        }

        public override void SetView(SceneView view)
        {
            base.SetView(view);
            
            mView.GenerateStageNodes(MapNodes);

            for (var i = 0; i < MapNodes.Count - 1; i++)
            {
                for (var j = 0; j < MapNodes[i].Count; j++)
                {
                    var curNode = MapNodes[i][j];
                    if (curNode == null) continue;
                    foreach (var nextNode in curNode.Model.NextNodes)
                    {
                        var targetNode = MapNodes[i + 1]
                            .Find(target => target != null && target.Model == nextNode);
                        if (targetNode == null) continue;

                        mView.GeneratePath(curNode.View, targetNode.View);
                    }
                }
            }

            ActiveNextNodes();
        }
        

        public void ActiveNextNodes()
        {
            CurNode.ClearMapNode();
            foreach (var nextNode in CurNode.Model.NextNodes)
            {
                var targetNode = MapNodes[CurStep+1].Find(target => target != null && target.Model == nextNode);
                if (targetNode == null) continue;

                mView.ActivateNextNodes(targetNode.View);
            }
        }
        public async UniTask SelectNode(MapNode mapNode)
        {
            CurNode = mapNode;
            CurStep = CurNode.Model.Step;
            foreach (var node in MapNodes[CurStep])
            {
                if(node == null || node == CurNode) continue;
                node.CloseMapNode();
            }
            await GameManager.Instance.LoadStageScene(CurNode);
        }

        
    }
}
