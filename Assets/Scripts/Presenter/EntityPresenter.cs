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

        public void UpdateEntityInfo()
        {
            View.UpdateHp(Model.CurHp, Model.MaxHp);
            View.UpdateActionGauge(Model.CurActionGauge, Model.MaxActionGauge);
        }
    }
}
