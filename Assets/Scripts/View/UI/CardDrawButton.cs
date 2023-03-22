using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    public class CardDrawButton : MonoBehaviour
    {
        public Button button;
        private void Start()
        {
            button.onClick.AsObservable().Subscribe(async _ =>
            {
                await GameManager.Instance.CurStage.DrawCard();
            });
        }
    }
}
