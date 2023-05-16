using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;

namespace View
{
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
            GameManager.Instance.CreateFloatingText(goldAmount.ToString(), goldText.transform.position, TextType.Gold);
            var targetGold = currentGold + goldAmount;
            DOTween.To(() => currentGold, x => currentGold = x, targetGold, duration)
                .OnUpdate(() => goldText.text = currentGold.ToString())
                .SetEase(Ease.Linear);
        }
    }
}
