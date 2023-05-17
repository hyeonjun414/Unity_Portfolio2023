using System.Collections.Generic;
using DG.Tweening;
using Manager;
using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace View
{
    public class CardHolder : MonoBehaviour
    {
        private const int Null = -1;

        [Header("연결")] 
        public Physics2DRaycaster raycaster;
        public RectTransform rectTransform;
        public TargetArrow bezierCurveDrawer;
        public Transform discardPivot;
        public Transform deckPivot;
        public Transform handPivot;
        public TextMeshProUGUI deckCountText;
        public TextMeshProUGUI discardCountText;
        
        [Header("카드")] 
        public int selectedCardIndex;
        public int mouseOverCardIndex;
        public List<CardView> cards;
        public List<CardView> deckCards = new();
        public List<CardView> discardCards = new();
        
        [Header("카드 이동 관련")] 
        public float lerpTime;
        public float angularInterval;
        public float zInterval;
        public float distance;
        public float mouseOverInterval;
        public float mouseOverScale;
        public float mouseOverYSpacing;
        public float selectedScale;
        public float selectedYSpacing;

        Vector3 _targetPos;
        Quaternion _targetRot;
        Vector3 _targetScl;
        
        private void Awake()
        {
            mouseOverCardIndex = Null;
            selectedCardIndex = Null;
            
            raycaster = GameManager.Instance.raycaster;
        }

        private void Update()
        {
            ArrangeCards();
        }
        
        private void ArrangeCards()
        {
            var startAngle =  angularInterval * 0.5f * (cards.Count - 1);
            var lerpAmount = lerpTime * Time.deltaTime;

            

            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i].transform;
                
                var angle = startAngle + -angularInterval * i;
                var radian = angle * Mathf.Deg2Rad;
                
                var x = Mathf.Sin(-radian) * distance;
                var y = Mathf.Cos(radian) * distance - distance;
                
                if (i == mouseOverCardIndex)
                {
                    _targetPos = new Vector3(x, mouseOverYSpacing, zInterval);
                    _targetRot = Quaternion.identity;
                    _targetScl = Vector3.one * mouseOverScale; 
                }
                else if (i == selectedCardIndex)
                {
                    if (cards[i].GetCardType() is CardType.Attack)
                    {
                        _targetPos = new Vector3(0, selectedYSpacing, zInterval);
                        _targetRot = Quaternion.identity;
                        _targetScl = Vector3.one * selectedScale;
                    }
                    else
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rectTransform, Input.mousePosition, 
                            raycaster.eventCamera, out var mousePos);
                        
                        _targetPos = new Vector3(mousePos.x, mousePos.y, zInterval);
                        _targetRot = Quaternion.identity;
                        _targetScl = Vector3.one * selectedScale; 
                    }
                }
                else
                {
                    _targetPos = new Vector3(x, y, i * -zInterval);
                    _targetRot = Quaternion.Euler(0, 0, angle);
                    _targetScl = Vector3.one;
                    
                    if (mouseOverCardIndex != Null)
                        _targetPos.x += (i < mouseOverCardIndex ? -1 : 1) * mouseOverInterval;
                }

                card.localPosition = Vector3.Lerp(card.localPosition, _targetPos, lerpAmount); 
                card.localRotation = Quaternion.Lerp(card.localRotation, _targetRot, lerpAmount);
                card.localScale = Vector3.Lerp(card.localScale, _targetScl, lerpAmount);
                
                card.SetSiblingIndex(i);
            }
            
            if(mouseOverCardIndex != Null)
                cards[mouseOverCardIndex].transform.SetAsLastSibling();
            if(selectedCardIndex != Null)
                cards[selectedCardIndex].transform.SetAsLastSibling();
        }


        public void DrawCard(CardView card)
        {
            var trans = card.transform;
            trans.SetParent(handPivot);
            trans.SetAsFirstSibling();
            deckCards.Remove(card);
            cards.Insert(0, card);
            UpdateCardCount();
        }
        
        public void DiscardCard(CardView cardView)
        {
            var card = cards.Find(t => t == cardView);
            cards.Remove(card);
            discardCards.Add(card);
            
            var trans = card.gameObject.transform;
            trans.SetParent(discardPivot);
            trans.DOMove(discardPivot.position, 0.5f).SetEase(Ease.OutExpo);
            trans.DOLocalRotate(new Vector3(0, 180, Random.Range(-10, 10)), 0.5f).SetEase(Ease.OutQuart);
            trans.DOScale(0.5f, 0.5f).SetEase(Ease.OutExpo);
            UpdateCardCount();
        }

        public void CardHovered(CardView cardView)
        {
            mouseOverCardIndex = cards.IndexOf(cardView);
        }

        public void CardUnHovered(CardView cardView)
        {
            var curCardIndex = cards.IndexOf(cardView);
            if (mouseOverCardIndex == curCardIndex)
                mouseOverCardIndex = Null;
        }

        public void CardSelected(CardView cardView)
        {
            mouseOverCardIndex = Null;
            selectedCardIndex = cards.IndexOf(cardView);
            if (cardView.GetCardType() == CardType.Attack)
            {
                bezierCurveDrawer.SetStartPoint(cardView.transform);
                bezierCurveDrawer.gameObject.SetActive(true);
            }
        }

        public void CardUnSelected()
        {
            selectedCardIndex = Null;
            bezierCurveDrawer.gameObject.SetActive(false);
        }

        public void AddCard(CardView cardView)
        {
            var trans = cardView.transform;
            trans.rotation = Quaternion.Euler(0, 180, 0);
            trans.DOScale(0.5f, 0.2f);
            trans.SetParent(deckPivot, false);
            cardView.transform.DOLocalMove(-Vector3.up * deckCards.Count * 3, 0.3f)
                .SetEase(Ease.OutQuart);
            cardView.transform.DOLocalRotate(new Vector3(0, 180, deckCards.Count), 0.3f);
            trans.SetAsLastSibling();
            deckCards.Add(cardView);
            UpdateCardCount();
        }

        public void ReturnToDeck(CardView cardView)
        {
            discardCards.Remove(cardView);
            deckCards.Add(cardView);
            
            cardView.transform.SetParent(deckPivot);
            cardView.transform.DOLocalMove(-Vector3.up * deckCards.Count * 3, 0.3f).SetEase(Ease.OutQuart);
            cardView.transform.DOLocalRotate(new Vector3(0, 180, deckCards.Count), 0.3f);
            cardView.transform.SetAsLastSibling();
            UpdateCardCount();
        }

        public void UpdateCardCount()
        {
            deckCountText.text = deckCards.Count.ToString();
            discardCountText.text = discardCards.Count.ToString();
        }
    }
}
