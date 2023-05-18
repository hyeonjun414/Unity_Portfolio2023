using System;
using Manager;
using Model;
using Presenter;
using UnityEngine;

namespace View
{
    public class UserView : SceneView
    {
        public User Presenter;

        public CardView cardPrefab;
        public UserGoldView userGoldView;
        public UserDeckView userDeckView;
        public ArtifactView artifactPrefab;
        public Transform artifactPivot;

        private void Start()
        {
            if (GameManager.Instance == null)
                return;

            Presenter = GameManager.Instance.User;
            Presenter.View = this;
            Presenter.Init();
        }

        public void SetView(User user)
        {
            Presenter = user;
            
            userGoldView.Init(user.Gold);
            foreach (var card in user.Cards)
            {
                card.View = CreateDeckCard();
                card.Init();
            }
            foreach (var artifact in user.Artifacts)
            {
                artifact.View = CreateArtifactView();
                artifact.Init(Presenter);
            }
        }

        public CardView CreateDeckCard()
        {
            var inst =  Instantiate(cardPrefab, userDeckView.transform);
            userDeckView.AddCard(inst);
            return inst;
        }

        public ArtifactView CreateArtifactView()
        {
            var inst = Instantiate(artifactPrefab, artifactPivot);
            return inst;
        }

        public void AddGold(int prevGold, int amount)
        {
            userGoldView.AddGold(prevGold, amount);
        }

        
    }
}
