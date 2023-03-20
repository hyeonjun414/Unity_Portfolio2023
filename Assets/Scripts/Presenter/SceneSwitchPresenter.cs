using Cysharp.Threading.Tasks;
using Model;
using View;

namespace Presenter
{
    public class SceneSwitchPresenter
    {
        public SceneSwitchModel Model;
        public SceneSwitchView View;

        public SceneSwitchPresenter(SceneSwitchModel model, SceneSwitchView view)
        {
            this.Model = model;
            this.View = view;
        }
        public async UniTask AsyncSceneLoad(string sceneName)
        {
            await View.FadeOut();
            await Model.AsyncSceneLoad(sceneName);
            await View.FadeIn();
        }
    }
}
