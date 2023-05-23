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
            userGoldView.Init(user.uModel.Gold);
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
