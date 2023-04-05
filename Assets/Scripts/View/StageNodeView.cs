using Manager;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class StageNodeView : MonoBehaviour
    {
        public Stage Presenter;

        public Button button;
        public void Init(Stage stage)
        {
            Presenter = stage;

            button.onClick.AsObservable().Subscribe(async _ =>
            {
                await Presenter.LoadStage();
            });
        }
    }
}
