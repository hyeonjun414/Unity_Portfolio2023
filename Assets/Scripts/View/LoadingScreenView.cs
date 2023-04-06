using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using Model;
using Presenter;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private Image loadingScreen;

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
