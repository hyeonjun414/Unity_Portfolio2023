using DG.Tweening;
using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class CardView : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public CardPresenter Presenter;

        public Image CardImage;
        public TextMeshProUGUI Text_Name;
        public TextMeshProUGUI Text_Desc;
        public TextMeshProUGUI Text_Damage;

        public void SetView(CardPresenter card)
        {
            Presenter = card;
            var data = card.Model;
            Text_Name.SetText(data.Name);
            Text_Desc.SetText(data.Desc);
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }
        
        public void Selected()
        {
            transform.DOScale(1.2f, 0.1f);
            transform.DOMoveY(50, 0.1f).SetRelative();
        }

        public void UnSelected()
        {
            transform.DOScale(1f, 0.1f);
            transform.DOMoveY(-50, 0.1f).SetRelative();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Presenter.SelectCard();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Presenter.UnSelectCard();
        }
    }
}
