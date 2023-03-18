using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class UserPresenter
    {
        public UserModel Model;
        public UserView View;

        public UserPresenter(UserModel model, UserView view)
        {
            this.Model = model;
            this.View = view;
        }

        public EntityModel GetHero()
        {
            return Model.Hero;
        }

        public void SetHero(EntityModel hero)
        {
            throw new System.NotImplementedException();
        }
    }
}
