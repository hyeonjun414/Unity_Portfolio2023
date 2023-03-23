using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class RewardView : MonoBehaviour
    {
        public Button skipButton;

        private void Start()
        {
            skipButton.onClick.AsObservable().Subscribe(async _ =>
            {
                await GameManager.Instance.CurStage.CloseReward(null);
            });
        }
    }
}
