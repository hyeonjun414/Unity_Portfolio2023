using System;
using System.Collections.Generic;
using Manager;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class RewardView : MonoBehaviour
    {
        public Reward Presenter;
        public Button skipButton;

        public Transform rewardPivot;
        public CardView cardPrefabs;

        private List<CardView> cardInstances = new();

        public void Init(Reward reward)
        {
            Presenter = reward;
            skipButton.onClick.AsObservable().Subscribe(async _ => { await Presenter.Close(); });
            for (var i = 0; i < reward.Cards.Count; i++)
            {
                var cardInst = Instantiate(cardPrefabs, rewardPivot);
                cardInstances.Add(cardInst);
                cardInst.SetView(reward.Cards[i]);
            }
        }
    }
}
