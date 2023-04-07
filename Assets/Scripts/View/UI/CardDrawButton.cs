using System;
using Manager;
using Presenter;
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
            // button.onClick.AsObservable().Subscribe(async _ =>
            // {
            //     var stage = GameManager.Instance.CurStage as BattleStage;
            //     if (stage != null)
            //     {
            //         await stage.DrawCard();
            //     }
            //     
            // });
        }
    }
}
