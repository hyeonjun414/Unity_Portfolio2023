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
        public Button skipButton;

        public Transform rewardPivot;
        public RewardCardView cardPrefabs;

        private List<RewardCardView> cardInstances = new();
        private void Start()
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                skipButton.onClick.AsObservable().Subscribe(async _ =>
                {
                    await curStage.CloseReward(null);
                });
            }
        }

        public void Init(Reward reward)
        {
            for (var i = 0; i < reward.Cards.Count; i++)
            {
                var cardInst = Instantiate(cardPrefabs, rewardPivot);
                cardInstances.Add(cardInst);
                cardInst.SetView(reward.Cards[i]);
            }
        }
    }
}
