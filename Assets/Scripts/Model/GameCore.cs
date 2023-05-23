using Manager;
using Presenter;

namespace Model
{
    public class GameCore
    {
        public User User;
        public Scene Scene;
        public Scene CurScene;

        public void Init()
        {
        }
        
        public void Load(GameManager gameManager)
        {
            User.Load(gameManager);
        }

        public void OpenScene(Scene scene)
        {
            if (Scene == null)
            {
                Scene = scene;
                CurScene = Scene;
                return;
            }
            CurScene.SetChild(scene);
            scene.SetParent(CurScene);
            CurScene = scene;
        }

        public void CloseCurScene()
        {
            if (CurScene == null) return;

            CurScene.CloseScene();
            CurScene = CurScene.Parent;
            CurScene.SceneActive(true);
            CurScene.Child = null;
        }
    }
}
