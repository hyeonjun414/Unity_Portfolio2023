using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Presenter;

namespace Model
{
    public class GameCore
    {
        public SeedRandom Rand;
        public User User;
        public List<Scene> Scenes;
        
        public Scene CurScene => Scenes.Last();

        public GameCore()
        {
            Scenes = new List<Scene>();
        }

        public void Init()
        {
            Rand = new SeedRandom();
            Rand.SetSeedFromSystemTime();
        }
        
        public void Load(GameManager gameManager)
        {
            for (int i = 0; i < Scenes.Count; i++)
            {
                var scene = Scenes[i];
                scene.Load(gameManager);
                scene.SceneActive(false);
                scene.SetLayerOrder(i);
            }
            CurScene.SceneActive(true);
        }
        
        public void OpenScene(Scene scene)
        {
            
            if (Scenes.Count != 0)
            {
                if(scene.View.isModal == false)
                    CurScene.SceneActive(false);
            }
            Scenes.Add(scene);
            scene.SetLayerOrder(Scenes.Count);
        }

        public void CloseCurScene()
        {
            CurScene.CloseScene();
            Scenes.Remove(CurScene);
            CurScene.SceneActive(true);
        }

        public void Reset()
        {
            SoundManager.Instance.SaveVolumeData();
            foreach (var scene in Scenes)
            {
                scene.CloseScene();
            }
            Scenes.Clear();
            Rand = null;
            User = null;
        }
    }
}
