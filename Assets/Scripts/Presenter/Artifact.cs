using Cysharp.Threading.Tasks;
using Model;
using View;

namespace Presenter
{
    public class Artifact : Item
    {
        public ArtifactModel Model;
        public ArtifactView View;
    
        public Artifact(ArtifactModel model, ArtifactView view)
        {
            Model = model;
            View = view;
        }

        public async UniTask Activate(ArtifactTrigger trigger, Stage stage, User user)
        {
            await Model.Activate(trigger, stage, user);
        }

        public void Init(User user)
        {
            Model.Init(user);
            View.SetView(this);
        }
    }

    public class ShopArtifact : Artifact
    {
        public ShopArtifact(ArtifactModel model, ArtifactView view) : base(model, view)
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
