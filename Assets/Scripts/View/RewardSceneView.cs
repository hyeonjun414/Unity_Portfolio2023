using System;
using System.Collections.Generic;
using Manager;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class RewardSceneView : SceneView
    {
        public Button skipButton;

        public Transform rewardPivot;
        public CardView cardPrefab;
        public ArtifactView artifactPrefab;
        public ArtifactDescPanel artifactDescPanel;
        
        public void SetView(RewardScene rewardScene)
        {
            skipButton.onClick.AsObservable().Subscribe(_ => { rewardScene.Close(); });
            foreach (var reward in rewardScene.Rewards)
            {
                switch (reward)
                {
                    case Card card:
                        var cardInst = Instantiate(cardPrefab, rewardPivot);
                        card.SetView(cardInst);
                        break;
                    case Artifact artifact:
                        var artifactInst = Instantiate(artifactPrefab, rewardPivot);
                        artifact.SetView(artifactInst);
                        break;
                }
            }
        }

        public void DisplayArtifactDesc(Artifact artifact)
        {
            artifactDescPanel.SetPanel(artifact);
        }
    }
}
