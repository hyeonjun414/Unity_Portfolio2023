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

        public UserGoldView userGoldView;
        public ArtifactView artifactPrefab;
        public Transform artifactPivot;

        public void SetView(User user)
        {
            Presenter = user;
            user.View = this;
            
            userGoldView.Init(user.Gold);
            foreach (var artifact in user.Artifacts)
            {
                AddArtifact(artifact);
            }
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
