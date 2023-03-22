using System;
using Model;
using Presenter;
using UnityEngine;

namespace View
{
    public interface IUserView
    {
        EntityModel GetHero();
    }
    public class UserView : MonoBehaviour, IUserView
    {
        public User Presenter;

       
        public EntityModel GetHero()
        {
            return Presenter.GetHero();
        }
    }
}
