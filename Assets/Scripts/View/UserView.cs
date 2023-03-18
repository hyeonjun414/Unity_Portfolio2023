using System;
using Model;
using Presenter;
using UnityEngine;

namespace View
{
    public interface IUserView
    {
        void SetHero(EntityModel hero);
        EntityModel GetHero();
    }
    public class UserView : MonoBehaviour, IUserView
    {
        public UserPresenter Presenter;

        public void SetHero(EntityModel hero)
        {
            Presenter.SetHero(hero);
        }

        public EntityModel GetHero()
        {
            return Presenter.GetHero();
        }
    }
}
