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

        public void SetView(User user)
        {
            Presenter = user;
            user.View = this;
            
            userGoldView.Init(user.Gold);
            foreach (var card in user.Cards)
            {
                card.View = CreateDeckCard();
                card.Init();
            }
            foreach (var artifact in user.Artifacts)
            {
                AddArtifact(artifact);
            }
        }

        public CardView CreateDeckCard()
        {
            var inst =  Instantiate(cardPrefab, userDeckView.transform);
            userDeckView.AddCard(inst);
            return inst;
        }

        public void AddGold(int prevGold, int amount)
        {
            userGoldView.AddGold(prevGold, amount);
        }

        public void AddArtifact(Artifact artifact)
        {
            var inst = Instantiate(artifactPrefab, artifactPivot);
            inst.SetView(artifact);
        }
    }
}
