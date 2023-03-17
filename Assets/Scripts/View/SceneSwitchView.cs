using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using Presenter;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class SceneSwitchView : MonoBehaviour
    {
        [SerializeField] private Image loadingScreen;
        public SceneSwitchPresenter presenter;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.sceneSwitchView = this;
                DontDestroyOnLoad(gameObject);

                if (presenter == null)
                {
                    presenter = new SceneSwitchPresenter();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public async UniTask AsyncSceneLoad(string sceneName)
        {
            loadingScreen.DOColor(Color.black, 0.5f)
                .OnStart(() => loadingScreen.gameObject.SetActive(true));
            await UniTask.Delay(500);
            await presenter.AsyncSceneLoad(sceneName);
            loadingScreen.DOColor(Color.clear, 0.5f)
                .OnComplete(() => loadingScreen.gameObject.SetActive(false));
            await UniTask.Delay(500);
        }
    }
}
