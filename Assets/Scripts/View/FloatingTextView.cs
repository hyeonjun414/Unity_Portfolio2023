using DG.Tweening;
using TMPro;
using UnityEngine;

namespace View
{
    public class FloatingTextView : MonoBehaviour
    {
        public TextMeshProUGUI text;

        public void SetFloatingText(string str, Vector3 position)
        {
            transform.position = position + (Vector3)Random.insideUnitCircle * 0.5f + Vector3.up * 1.5f;
            text.SetText(str);
            transform.DOPunchScale(Vector3.one, 0.5f, 5, 0.2f);
            transform.DOMoveY(0.5f, 1f).SetRelative();
            text.DOColor(Color.clear, 1f).SetEase(Ease.InExpo).OnComplete(() => Destroy(gameObject));
        }
    }
}
