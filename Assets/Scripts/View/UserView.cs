using System;
using Model;
using Presenter;
using UnityEngine;

namespace View
{
    public interface IUserView
    {
    }
    public class UserView : MonoBehaviour, IUserView
    {
        public User Presenter;

    }
}
