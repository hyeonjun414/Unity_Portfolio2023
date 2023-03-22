using Cysharp.Threading.Tasks;
using Model;
using View;

namespace Presenter
{
    public class SceneSwitcher
    {
        public SceneSwitcherModel Model;
        public SceneSwitcherView View;

        public SceneSwitcher(SceneSwitcherModel model, SceneSwitcherView view)
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
