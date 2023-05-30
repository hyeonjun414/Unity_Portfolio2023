using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using Newtonsoft.Json;
using UnityEngine;
using View;

namespace Presenter
{
    public class MapNode
    {
        public MapNodeModel Model;

        [JsonIgnore]
        public MapNodeView View;

        public MapNode()
        {
            
        }
        public MapNode(MapNodeModel model)
        {
            Model = model;
        }

        public void SetView(MapNodeView view)
        {
            View = view;
            View.Presenter = this;
            View.SetView(this);
        }

        public async UniTask SelectMapNode()
        {
            await GameManager.Instance.CurMap.SelectNode(this);
        }

        public void CloseMapNode()
        {
            View.ActiveNode(false);
        }

        public void ClearMapNode()
        {
            View?.ClearMapNode();
        }

        public string GetIconName() => Model.StageData.Icon;
        
    }
}
