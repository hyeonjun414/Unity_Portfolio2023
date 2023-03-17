using Cysharp.Threading.Tasks;
using Model;

namespace Presenter
{
    public class SceneSwitchPresenter
    {
        private SceneSwitchModel model;

        public SceneSwitchPresenter()
        {
            model = new SceneSwitchModel();
        }
        public async UniTask AsyncSceneLoad(string sceneName)
        {
            await model.AsyncSceneLoad(sceneName);
        }
    }
}
