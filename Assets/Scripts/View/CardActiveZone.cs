using Manager;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class CardActiveZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.EnterCardZone();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.ExitCardZone();
        }
    }
}
