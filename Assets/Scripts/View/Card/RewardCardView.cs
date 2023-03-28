using System;
using Manager;
using Presenter;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class RewardCardView : CardView, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            var stage = GameManager.Instance.CurStage as BattleStage;
            if (stage != null)
            {
                stage.CloseReward(Presenter);
            }
        }
    }
}
