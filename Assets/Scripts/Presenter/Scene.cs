using Manager;
using Model;
using Newtonsoft.Json;
using View;

namespace Presenter
{
    public class Scene
    {
        public string Type;

        public SceneModel Model;
        [JsonIgnore]
        public SceneView View;

        [JsonIgnore]
        public GameManager gm;

        public Scene()
        {
            Type = this.GetType().Name;
        }

        public Scene(GameManager gm, SceneModel model)
        {
            this.gm = gm;
            Model = model;
            Type = this.GetType().Name;
        }

        public virtual void Load(GameManager gameManager)
        {
            gm = gameManager;
            SetView(gm.CreateSceneView(this));
        }
        
        public void SceneActive(bool isActive)
        {
            View.SceneViewActive(isActive);
        }

        public void CloseScene()
        {
            Model = null;
            View.DestroyScene();
        }

        public virtual void SetView(SceneView view)
        {
            View = view;
            View.Presenter = this;
        }

        
    }
}
