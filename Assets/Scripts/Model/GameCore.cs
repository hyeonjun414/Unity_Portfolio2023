using System.Collections.Generic;
using System.Linq;
using Manager;
using Newtonsoft.Json;
using Presenter;

namespace Model
{
    public class GameCore
    {
        public User User;
        public List<Scene> Scenes;

        public Scene CurScene => Scenes.Last();

        public GameCore()
        {
            Scenes = new List<Scene>();
        }

        public void Init()
        {
        }
        
        public void Load(GameManager gameManager)
        {
            foreach (var scene in Scenes)
            {
                scene.Load(gameManager);
                scene.SceneActive(false);
            }
            CurScene.SceneActive(true);
        }
        
        public void OpenScene(Scene scene)
        {
            if (Scenes.Count != 0)
            {
                CurScene.SceneActive(false);
            }
            Scenes.Add(scene);
        }

        public void CloseCurScene()
        {
            CurScene.CloseScene();
            Scenes.Remove(CurScene);
            CurScene.SceneActive(true);
        }
    }
}
