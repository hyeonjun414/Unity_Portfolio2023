using System.Collections.Generic;
using DG.Tweening;
using Model;
using TMPro;
using UnityEngine;

namespace View
{
    public enum TextType
    {
        Damage,
        Heal,
        Miss,
    }
    
    public class FloatingTextView : MonoBehaviour
    {
        public TextMeshProUGUI text;

        public List<Color> colors;
        public void SetFloatingText(string str, Vector3 position, TextType textType)
        {
            transform.position = position + (Vector3)Random.insideUnitCircle * 0.5f + Vector3.up * 1.5f;
            text.SetText(str);
            text.color = colors[(int)textType];
            transform.DOPunchScale(Vector3.one, 0.5f, 5, 0.2f);
            transform.DOMoveY(0.5f, 1f).SetRelative();
            text.DOColor(Color.clear, 1f).SetEase(Ease.InExpo).OnComplete(() => Destroy(gameObject));
        }
    }
}
