using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Model
{
    public class SceneSwitcher : MonoBehaviour
    {
        public Image loadingScreen;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.sceneSwitcher = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async UniTask AsyncSceneLoad(string sceneName)
        {
            loadingScreen.DOColor(Color.black, 0.5f)
                .OnStart(()=> loadingScreen.gameObject.SetActive(true));
            await UniTask.Delay(500);

            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }

                await UniTask.Yield();
            }

            loadingScreen.DOColor(Color.clear, 0.5f)
                .OnComplete(()=> loadingScreen.gameObject.SetActive(false));
            await UniTask.Delay(500);
        }
        
    }
}
