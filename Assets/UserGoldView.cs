using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UserGoldView : MonoBehaviour
{
    public Transform iconPivot;
    public TextMeshProUGUI goldText;

    public float riseSpeed = 1f;
    public float duration = 1f;
    
    public void Init(int gold)
    {
        goldText.text = gold.ToString();
    }

    public void AddGold(int currentGold, int goldAmount)
    {
        var targetGold = currentGold + goldAmount;
        DOTween.To(() => currentGold, x => currentGold = x, targetGold, duration)
            .OnUpdate(() => goldText.text = currentGold.ToString())
            .SetEase(Ease.Linear);
    }
}
