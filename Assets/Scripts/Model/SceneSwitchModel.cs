using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Model
{
    public class SceneSwitchModel
    {
        public async UniTask AsyncSceneLoad(string sceneName)
        {
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
        }
        
    }
}
