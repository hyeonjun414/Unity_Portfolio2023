using System.Threading.Tasks;
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

        public async UniTask SelectMapNode()
        {
            await GameManager.Instance.CurMap.SelectNode(this);
        }
    }
}
