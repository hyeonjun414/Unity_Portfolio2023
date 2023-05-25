using System;
using System.Collections.Generic;
using Manager;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class RewardView : SceneView
    {
        public Button skipButton;

        public Transform rewardPivot;
        public CardView cardPrefabs;
        
        public void SetView(Reward reward)
        {
            skipButton.onClick.AsObservable().Subscribe(_ => { reward.Close(); });
            foreach (var card in reward.Cards)
            {
                var cardInst = Instantiate(cardPrefabs, rewardPivot);
                card.SetView(cardInst);
            }
        }
    }
}
