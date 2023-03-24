using System;
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
        public CardView cardPrefabs;
        private void Start()
        {
            skipButton.onClick.AsObservable().Subscribe(async _ =>
            {
                await GameManager.Instance.CurStage.CloseReward(null);
            });
        }

        public void Init(Reward reward)
        {
            foreach (var card in reward.Cards)
            {
                var cardInst = Instantiate(cardPrefabs, rewardPivot);
                cardInst.SetView(card);
            }
        }
    }
}
