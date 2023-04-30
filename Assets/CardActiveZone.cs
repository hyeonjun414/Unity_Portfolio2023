using System.Collections;
using System.Collections.Generic;
using Manager;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardActiveZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        print("Enter CardZone");
        var curStage = GameManager.Instance.CurStage as BattleStage;
        curStage?.EnterCardZone();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("Exit CardZone");
        var curStage = GameManager.Instance.CurStage as BattleStage;
        curStage?.ExitCardZone();
    }
}
