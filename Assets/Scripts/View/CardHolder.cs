using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Manager;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

namespace View
{
    public class CardHolder : MonoBehaviour
    {
        private const int Null = -1;

        [Header("연결")] 
        [SerializeField] private Physics2DRaycaster raycaster; // 마우스 오버된 카드를 찾기 위한 레이케스터
        [SerializeField] private RectTransform rectTransform; // 카드 홀더의 RectTransform
        [SerializeField] private TargetArrow bezierCurveDrawer; // 베지어 커브 드로어
        private BattleStage battleStage; // 배틀 매니저

        [Header("오브젝트")] 
        [SerializeField] private HeroView player; // 플레이어 오브젝트
        [SerializeField] private EnemyView mouseOverEnemy; // 마우스 오버된 적 오브젝트

        [Header("카드 뽑기/버림 위치")] 
        [SerializeField] private Transform drawPosition; // 카드를 뽑는 위치
        [SerializeField] private Transform discardPosition; // 카드를 버리는 위치

        [Header("카드")] 
        [SerializeField] private int selectedCardIndex; // 선택된 카드 인덱스
        [SerializeField] private int mouseOverCardIndex; // 마우스 오버된 카드의 인덱스
        [SerializeField] private List<CardView> cards; // 카드 리스트

        [Header("일반 카드 수치")] 
        [SerializeField] private float lerpTime; // lerp 정도
        [SerializeField] private float angularInterval; // 각도 간격
        [SerializeField] private float zInterval; // z 간격
        [SerializeField] private float distance; // 삼각함수 계산 거리

        [Header("마우스 오버 카드 수치")] 
        [SerializeField] private float mouseOverInterval; // 마우스 오버시 다른 카드들이 양옆으로 밀리는 간격
        [SerializeField] private float mouseOverScale; // 마우스 오버된 카드의 스케일
        [SerializeField] private float mouseOverYSpacing;

        [Header("선택된 카드 수치")] 
        [SerializeField] private float selectedScale; // 선택된 카드의 스케일
        [SerializeField] private float selectedYSpacing; // 선택된 카드의 y 보정값

        private bool isControllable; // 현재 컨트롤 가능 여부

        public void SetControllable(bool controllable) => isControllable = controllable;

        private void Awake()
        {
            mouseOverEnemy = null;
            mouseOverCardIndex = Null;
            selectedCardIndex = Null;

            isControllable = false;

            raycaster = GameManager.Instance.raycaster;

        }

        private void Update()
        {
            ArrangeCards();
        }
        
        private void ArrangeCards()
        {
        // 시작 각도
        // *정 가운데 카드가 0도, 우측으로 갈수록 -각도, 좌측으로 갈수록 +각도가 된다. -> 각도가 반대이다.
        var startAngle =  angularInterval * 0.5f * (cards.Count - 1);
        // lerp 정도
        var lerpAmount = lerpTime * Time.deltaTime;

        // 목표 수치들
        Vector3 targetPos;
        Quaternion targetRot;
        Vector3 targetScl;

        // 카드 전체 순회하며 위치,각도,스케일 설정
        for (var i = 0; i < cards.Count; i++)
        {
            // 현재 선택된 카드
            var card = cards[i].transform;
            
            // 각도 -> 시작각도로부터 각도 간격만큼 떨어진 각도
            var angle = startAngle + -angularInterval * i;
            // 라디안 -> 각도를 라디안으로 변경한 값 (삼각함수 계산용)
            var radian = angle * Mathf.Deg2Rad;
            
            // 삼각함수를 이용한 위치 좌표 변환
            // *각도가 반대이므로 sin(-x) = -sin(x), cos(-x) = cos(x)를 적용한다.
            // *y의 경우, 원점에서 distance만큼 떨어지면 불편하므로, 다시 distance를 빼준다. 
            // x좌표 -> 삼각함수 sin값 (중심으로부터 원형으로 각도만큼 떨어진 좌표)
            var x = Mathf.Sin(-radian) * distance;
            // y좌표 -> 삼각함수 cos값 (중심으로부터 원형으로 각도만큼 떨어진 좌표)
            var y = Mathf.Cos(radian) * distance - distance;
            
            // 만약 마우스 오버된 카드라면,
            if (i == mouseOverCardIndex)
            {
                targetPos = new Vector3(x, mouseOverYSpacing, zInterval);
                targetRot = Quaternion.identity;
                targetScl = Vector3.one * mouseOverScale; 
            }
            // 만약 선택된 카드라면,
            else if (i == selectedCardIndex)
            {
                // 공격 카드라면,
                if (cards[i].GetCardType() is CardType.Attack)// or CardType.Magic)
                {
                    targetPos = new Vector3(0, selectedYSpacing, zInterval);
                    targetRot = Quaternion.identity;
                    targetScl = Vector3.one * selectedScale;
                }
                // 방어 카드라면,
                else
                {
                    // 마우스 위치를 구함
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rectTransform, Input.mousePosition, 
                        raycaster.eventCamera, out var mousePos);
                    
                    targetPos = new Vector3(mousePos.x, mousePos.y, zInterval);
                    targetRot = Quaternion.identity;
                    targetScl = Vector3.one * selectedScale; 
                }
            }
            // 만약 일반 홀딩 카드라면,
            else
            {
                targetPos = new Vector3(x, y, i * -zInterval);
                targetRot = Quaternion.Euler(0, 0, angle);
                targetScl = Vector3.one;
                
                // 마우스 오버 카드가 있을경우,
                if (mouseOverCardIndex != Null)
                    // 양옆으로 간격만큼 벌려줌
                    targetPos.x += (i < mouseOverCardIndex ? -1 : 1) * mouseOverInterval;
            }

            // Lerp로 최종 위치,각도,스케일 설정
            card.localPosition = Vector3.Lerp(card.localPosition, targetPos, lerpAmount); 
            card.localRotation = Quaternion.Lerp(card.localRotation, targetRot, lerpAmount);
            card.localScale = Vector3.Lerp(card.localScale, targetScl, lerpAmount);
            
            // Transform 순서를 설정하여 뒷 카드는 뒤에, 앞 카드는 앞에 그려줌
            card.SetSiblingIndex(i);
        }
        
        // 마우스 오버 카드가 있을경우,
        if(mouseOverCardIndex != Null)
            // 마우스 오버 카드를 맨 앞에 배치
            cards[mouseOverCardIndex].transform.SetAsLastSibling();
        // 선택된 카드가 있을경우,
        if(selectedCardIndex != Null)
            // 선택된 카드를 맨 앞에 배치
            cards[selectedCardIndex].transform.SetAsLastSibling();
        }


        public void DrawCard(CardView card)
        {
            var trans = card.transform;
            trans.SetParent(transform);
            trans.SetAsFirstSibling();

            cards.Insert(0, card);
        }
        
        public void DiscardCard(CardView cardView)
        {
            var card = cards.Find(t => t == cardView);
            cards.Remove(card);
            
            var trans = card.gameObject.transform;
            trans.SetParent(discardPosition);
            trans.DOMove(discardPosition.position, 0.5f).SetEase(Ease.OutExpo);
            trans.DOLocalRotate(new Vector3(0, 180, Random.Range(-25, 25)), 0.5f).SetEase(Ease.OutQuart);
            trans.DOScale(0.5f, 0.5f).SetEase(Ease.OutExpo);
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
    }
}
