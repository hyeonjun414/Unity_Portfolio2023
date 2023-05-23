using Cysharp.Threading.Tasks;
using Model;
using View;

namespace Presenter
{
    public class Artifact : Item
    {
        public ArtifactModel Model;
        public ArtifactView View;
        public string Id => Model.Id;
    
        public Artifact(ArtifactModel model)
        {
            Model = model;
            Init();
        }

        public async UniTask Activate(ArtifactTrigger trigger, object target)
        {
            await Model.Activate(trigger, target);
        }

        public void InitFunc(User user)
        {
            Model.Init(user);
        }

        public void Init()
        {
            
        }

        public void SetView(ArtifactView view)
        {
            View = view;
            View.Presenter = this;
            View.SetView(this);
        }

        
    }

    public class ShopArtifact : Artifact
    {
        public ShopArtifact(ArtifactModel model) : base(model)
        {
        }

        public void Sold()
        {
            if (View is ShopArtifactView shopArtifactView)
            {
                shopArtifactView.Sold();
            }
        }
    }
}
