using System;
using Model;
using UnityEngine;
using GameMasterPresenter = Presenter.GameMasterPresenter;

namespace View
{
    public class GameMasterView : MonoBehaviour
    {
        public static GameMasterView Instance { get; private set; }
        public SceneSwitchView sceneSwitchView;
        public GameMasterPresenter Presenter;
        public MasterTable MasterTable => Presenter.GetMasterTable();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Presenter = new GameMasterPresenter(new GameMasterModel(), this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public UserModel GetUser()
        {
            return Presenter.GetUser();
        }
    }
}
