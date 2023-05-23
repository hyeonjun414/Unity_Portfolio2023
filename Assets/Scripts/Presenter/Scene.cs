using Manager;
using Model;
using View;

namespace Presenter
{
    public class Scene
    {
        public Scene Parent;
        public Scene Child;

        public SceneModel Model;
        public SceneView View;
        
        public GameManager gm;

        public Scene(GameManager gm, SceneModel model)
        {
            this.gm = gm;
            Model = model;
        }
        
        public void SetParent(Scene parent)
        {
            Parent = parent;
            Parent.SceneActive(false);
        }
        public void SetChild(Scene child)
        {
            Child = child;
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
