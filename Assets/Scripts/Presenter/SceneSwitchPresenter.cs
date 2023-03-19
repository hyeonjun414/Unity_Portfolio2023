using Cysharp.Threading.Tasks;
using Model;
using View;

namespace Presenter
{
    public class SceneSwitchPresenter
    {
        private SceneSwitchModel model;
        private SceneSwitchView view;

        public SceneSwitchPresenter(SceneSwitchModel model, SceneSwitchView view)
        {
            this.model = model;
            this.view = view;
        }
        public async UniTask AsyncSceneLoad(string sceneName)
        {
            await view.FadeOut();
            await model.AsyncSceneLoad(sceneName);
            await view.FadeIn();
        }
    }
}
