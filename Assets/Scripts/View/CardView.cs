using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        public Card Presenter;

        public Image CardImage;
        public GameObject front;
        public GameObject back;
        public TextMeshProUGUI Text_Name;
        public TextMeshProUGUI Text_Desc;
        public TextMeshProUGUI Text_Damage;
        public ParticleSystem CardEffect;

        private void Update()
        {
            var forward = transform.forward;
            front.SetActive(forward.z > 0);
            back.SetActive(forward.z <= 0);
        }

        public void SetView(Card card)
        {
            Presenter = card;
            var data = card.Model;
            Text_Name.SetText(data.Name);
            Text_Desc.SetText(data.Desc);
            CardEffect = Resources.Load<ParticleSystem>($"Particle/{data.Effect}");
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

        public async UniTask PlayCardEft(EntityView ev)
        {
            var eft = Instantiate(CardEffect, ev.transform);
            eft.transform.position += Vector3.up;
            Destroy(eft.gameObject, eft.main.duration);
            await UniTask.Yield();
        }
    }
}
