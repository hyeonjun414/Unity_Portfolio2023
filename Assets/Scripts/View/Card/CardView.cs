using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            Presenter = card;
            Presenter.View = this;
            
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

        
        public void OnPointerClick(PointerEventData eventData)
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

        public void Hovered(ICardState cardState)
        {
            switch (cardState)
            {
                case CardBattleState:
                    content.DOScale(1.2f, 0.1f);
                    content.DOLocalMoveY(50, 0.1f);
                    break;
                case CardRewardState:
                    content.DOScale(1.2f, 0.1f);
                    break;
            }
        }

        public void Unhovered(ICardState cardState)
        {
            switch (cardState)
            {
                case CardBattleState :
                    content.DOScale(1f, 0.1f);
                    content.DOLocalMoveY(0, 0.1f);
                    break;
                case CardRewardState :
                    content.DOScale(1f, 0.1f);
                    break;
            }
        }
        public void Selected(ICardState cardState)
        {
            switch (cardState)
            {
                case CardBattleState:
                    content.DOScale(1.3f, 0.1f);
                    content.DOLocalMoveY(75, 0.1f);
                    break;
                case CardRewardState:
                    content.DOScale(1.3f, 0.1f);
                    break;
            }
        }

        public void UnSelected(ICardState cardState)
        {
            RollBack();
        }

        public void RollBack()
        {
            content.SetParent(transform);
            content.DOLocalMove(Vector3.zero, 0.1f);
            content.DOScale(1f, 0.1f);
        }

        public async UniTask PlayCardEft(CharacterView ev)
        {
            var eft = Instantiate(CardEffect, ev.transform);
            eft.transform.position += Vector3.up;
            Destroy(eft.gameObject, eft.main.duration);
            await UniTask.Yield();
        }


        public CardType GetCardType()
        {
            return Presenter.GetCardType();
        }
    }
}
