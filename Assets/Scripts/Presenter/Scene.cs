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
        protected GameManager gm;

        public Scene()
        {
            Type = this.GetType().Name;
        }

        public Scene(SceneModel model)
        {
            this.gm = GameManager.Instance;
            Model = model;
            Type = this.GetType().Name;
        }

        public virtual void Init()
        {
        }

        public virtual void Load(GameManager gameManager)
        {
            gm = gameManager;
            SetView(gm.CreateSceneView(this));
        }

        public virtual void SetView(SceneView view)
        {
            View = view;
            View.Presenter = this;
        }
        
        public virtual void ActivateScene(bool isActive)
        {
            View.SceneActivated(isActive);
        }

        public virtual void CloseScene()
        {
            Model = null;
            View.SceneClosed();
        }
        public virtual void SetLayerOrder(int orderNum)
        {
            View.SetLayerOrder(orderNum);
        }
    }
}
