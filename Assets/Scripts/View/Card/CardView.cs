using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace View
{
    public class CardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
        IPointerUpHandler, IPointerDownHandler
    {
        public Card Presenter;

        public Image CardImage;
        public Transform content;
        public GameObject front;
        public GameObject back;
        public TextMeshProUGUI Text_Name;
        public TextMeshProUGUI Text_Desc;
        public TextMeshProUGUI Text_Cost;
        public ParticleSystem CardEffect;

        public Image typeIcon;
        public List<Sprite> typeImages;

        public Image inputChecker;

        private void Update()
        {
            var forward = transform.forward;
            front.SetActive(forward.z > 0);
            back.SetActive(forward.z <= 0);
        }
        
        public virtual void SetView(Card card)
        {
            var data = card.Model;
            Text_Name.SetText(data.Name);
            Text_Desc.SetText(data.Desc);
            Text_Cost.SetText(data.Cost.ToString());
            CardImage.sprite = Resources.Load<Sprite>($"CardImage/{data.Icon}");
            CardEffect = Resources.Load<ParticleSystem>($"Particle/{data.Effect}");

            typeIcon.sprite = typeImages[(int)GetCardType()];
            SetInputChecker(true);
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }

        public void SetInputChecker(bool value) => inputChecker.raycastTarget = value;

        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Presenter.OnClick();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Presenter.OnHover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Presenter.OnUnhover();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Presenter.OnClickUp();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Presenter.OnClickDown();
        }

        public void Hovered(IItemState itemState)
        {
            content.DOScale(1.2f, 0.1f);
        }

        public void Unhovered(IItemState itemState)
        {
            content.DOScale(1f, 0.1f);
        }
        public void Selected(IItemState itemState)
        {
            content.DOScale(1.3f, 0.1f);
        }

        public void UnSelected(IItemState itemState)
        {
            RollBack();
        }

        public void RollBack()
        {
            content.SetParent(transform);
            content.DOLocalMove(Vector3.zero, 0.1f);
            content.DOScale(1f, 0.1f);
        }

        public CardType GetCardType()
        {
            return Presenter.GetCardType();
        }
    }
}
