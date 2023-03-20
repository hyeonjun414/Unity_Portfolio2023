using Cysharp.Threading.Tasks;
using DG.Tweening;
using Model;
using Presenter;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class SceneSwitchView : MonoBehaviour
    {
        [SerializeField] private Image loadingScreen;
        public SceneSwitchPresenter Presenter;

        private void Start()
        {
            if (Presenter == null)
            {
                Presenter = GameManager.Instance.SceneSwitchPresenter;
                Presenter.View = this;
            }
        }

        public async UniTask FadeOut()
        {
            loadingScreen.DOColor(Color.black, 0.5f)
                .OnStart(() => loadingScreen.gameObject.SetActive(true));
            await UniTask.Delay(500);
        }

        public async UniTask FadeIn()
        {
            loadingScreen.DOColor(Color.clear, 0.5f)
                .OnComplete(() => loadingScreen.gameObject.SetActive(false));
            await UniTask.Delay(500);
        }
    }
}
