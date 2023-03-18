using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class EntityPresenter
    {
        public EntityModel Model;
        public EntityView View;

        public EntityPresenter(EntityModel model, EntityView view)
        {
            Model = model;
            View = view;
        }
    }
}
