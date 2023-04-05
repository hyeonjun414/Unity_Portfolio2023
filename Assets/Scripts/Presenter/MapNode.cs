using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class MapNode
    {
        public MapNodeModel Model;
        public MapNodeView View;

        public MapNode(MapNodeModel model, MapNodeView view)
        {
            Model = model;
            View = view;
        }

        public virtual async UniTask LoadStage()
        {
            await GameManager.Instance.LoadStageScene(this);
        }
    }
}
