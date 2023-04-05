using Manager;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class MapNodeView : MonoBehaviour
    {
        public MapNode Presenter;

        public Button button;
        public void Init(MapNode mapNode)
        {
            Presenter = mapNode;
            Presenter.View = this;

            button.onClick.AsObservable().Subscribe(async _ =>
            {
                await Presenter.LoadStage();
            });
        }
    }
}
