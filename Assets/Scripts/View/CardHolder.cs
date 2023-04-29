using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

namespace View
{
    public class CardHolder : MonoBehaviour
    {
        private const int Null = -1;

        [Header("연결")] [SerializeField] private Physics2DRaycaster raycaster; // 마우스 오버된 카드를 찾기 위한 레이케스터
        [SerializeField] private RectTransform rectTransform; // 카드 홀더의 RectTransform
        [SerializeField] private TargetArrow bezierCurveDrawer; // 베지어 커브 드로어
        [SerializeField] private BattleStage battleStage; // 배틀 매니저

        [Header("오브젝트")] [SerializeField] private HeroView player; // 플레이어 오브젝트
        [SerializeField] private EnemyView mouseOverEnemy; // 마우스 오버된 적 오브젝트

        [Header("카드 뽑기/버림 위치")] [SerializeField]
        private Transform drawPosition; // 카드를 뽑는 위치

        [SerializeField] private Transform discardPosition; // 카드를 버리는 위치

        [Header("카드")] [SerializeField] private int selectedCardIndex; // 선택된 카드 인덱스
        [SerializeField] private int mouseOverCardIndex; // 마우스 오버된 카드의 인덱스
        [SerializeField] private List<CardView> cards; // 카드 리스트

        [Header("일반 카드 수치")] [SerializeField] private float lerpTime; // lerp 정도
        [SerializeField] private float angularInterval; // 각도 간격
        [SerializeField] private float zInterval; // z 간격
        [SerializeField] private float distance; // 삼각함수 계산 거리

        [Header("마우스 오버 카드 수치")] [SerializeField]
        private float mouseOverInterval; // 마우스 오버시 다른 카드들이 양옆으로 밀리는 간격

        [SerializeField] private float mouseOverScale; // 마우스 오버된 카드의 스케일
        [SerializeField] private float mouseOverYSpacing;

        [Header("선택된 카드 수치")] [SerializeField] private float selectedScale; // 선택된 카드의 스케일
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
            MouseOverDetection();
            
            if (isControllable) MouseClickDetection();

            ArrangeCards();

            EnemySelectionTask();
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
                if (cards[i].GetCardType() == CardType.Attack)
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

        private void EnemySelectionTask()
        {
            // 선택된 카드가 있고, 그 카드가 공격 카드라면,
            if (selectedCardIndex != Null &&
                cards[selectedCardIndex].GetCardType() == CardType.Attack)
            {
                // 베지어 곡선의 시작점을 선택된 카드의 위치로 설정
                bezierCurveDrawer.SetStartPoint(cards[selectedCardIndex].transform.position);

                // 마우스 오버된 적이 있다면 베지어 곡선을 강조하고, 없다면 강조 안함
                bezierCurveDrawer.SetHighLight(mouseOverEnemy != null);
            }
        }

        private void MouseOverDetection()
        {
            // Physics2DRaycaster를 이용하여 화면상에서 마우스 오버된 UI 오브젝트를 레이캐스트한다.
            var eventData = new PointerEventData(null) { position = Input.mousePosition };
            var results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            // 마우스 오버된 카드, 적을 초기화한다.
            mouseOverCardIndex = Null;
            mouseOverEnemy = null;

            // 마우스 오버된 오브젝트가 있을 경우,
            if (results.Count > 0)
            {
                // 태그 비교로 카드/에너미 리스트로 분리한다.
                var cardResults = results.Where(x => x.gameObject.CompareTag("Card")).ToList();
                var enemyResults = results.Where(x => x.gameObject.CompareTag("Enemy")).ToList();
            
                // 선택된 카드가 없는 상태에서, 마우스 오버된 카드가 있을경우,
                if (selectedCardIndex == Null && cardResults.Count > 0)
                {
                    // 카드중 가장 가까운 카드를 선택하여 (카드는 겹치므로)
                    var result = cardResults.Aggregate((a, b) => a.distance > b.distance ? a : b);
                    // 해당 카드의 인덱스를 얻는다.
                    Debug.Log(result.gameObject.GetComponent<CardView>());
                    mouseOverCardIndex = cards.IndexOf(result.gameObject.GetComponent<CardView>());
                }
                // 선택된 카드가 있는 상태에서, 마우스 오버된 적이 있을경우, 
                else if (selectedCardIndex != Null && enemyResults.Count > 0)
                {
                    // 해당 적을 얻어온다. (적은 겹치지 않는다)
                    mouseOverEnemy = enemyResults[0].gameObject.GetComponent<EnemyView>();
                }
            }
        }

        private void MouseClickDetection()
        {
            // 좌클릭시,
            if (Input.GetMouseButtonDown(0))
            {
                // 카드 선택 (마우스 오버된 카드가 있을 경우에만)
                if (mouseOverCardIndex != Null)
                {
                    // 선택 카드 설정
                    selectedCardIndex = mouseOverCardIndex;
                    // 마우스 오버 카드 초기화
                    mouseOverCardIndex = Null;
                    // 효과음 재생
                    //AudioManager.PlaySE(AudioManager.SE.SelectCard);

                    // 공격 카드일 경우,
                    if (cards[selectedCardIndex].GetCardType() == CardType.Attack)
                    {
                        //베지어 라인 활성화
                        bezierCurveDrawer.gameObject.SetActive(true);
                    }
                        
                }
                // 카드 사용 (선택된 카드가 있을 경우에만)
                else if (selectedCardIndex != Null)
                {
                    TryUseCard();
                }
            }
            // 우클릭시,
            else if (Input.GetMouseButtonDown(1))
            {
                // 선택된 카드 초기화
                selectedCardIndex = Null;
                // 베지어 라인 비활성화
                bezierCurveDrawer.gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonUp(0))
            {
                // 선택된 카드 초기화
                selectedCardIndex = Null;
                // 베지어 라인 비활성화
                bezierCurveDrawer.gameObject.SetActive(false);
            }
        }

        private void TryUseCard()
        {
            var card = cards[selectedCardIndex];

            // 에너지가 부족할 경우 종료
            // if (card.CardCost > battleManager.energy)
            // {
            //     battleManager.EnergyShortage();
            //     return;
            // }
            
            // 공격 카드인데, 마우스 오버된 적이 없을경우 종료
            if (card.GetCardType() ==CardType.Attack && mouseOverEnemy == null)
                return;
            
            // 에너지 소모
            //battleManager.energy -= card.CardCost;
            // 카드 사용
            //cards[selectedCardIndex].Use(player, mouseOverEnemy);
            // 사용한 카드 버림
            //DiscardCard(selectedCardIndex);
            // 선택된 카드 초기화
            selectedCardIndex = Null;
            // 베지어 라인 비활성화
            bezierCurveDrawer.gameObject.SetActive(false);
        }

        public void DrawCard(CardView card)
        {
            var cardTransform = card.transform;

            // 카드 부모를 카드 홀더로 설정
            cardTransform.SetParent(transform);
            // 카드 순서를 맨 처음으로 설정 
            cardTransform.SetAsFirstSibling();

            // 카드 위치/회전/스케일을 설정
            cardTransform.position = drawPosition.position;
            cardTransform.rotation = drawPosition.rotation;
            cardTransform.localScale = drawPosition.localScale;

            // 카드 활성화
            cardTransform.gameObject.SetActive(true);
            // 카드 리스트에 추가
            cards.Insert(0, card);

            // 효과음 재생
            //AudioManager.PlaySE(AudioManager.SE.DrawCard);
        }

        private void DiscardCard(int cardIndex)
        {
            var card = cards[cardIndex];

            // 카드를 리스트에서 지움
            cards.Remove(card);
            // 버린 카드더미에 추가
            //battleManager.AddToDiscardPile(card);

            // 효과음 재생
            //AudioManager.PlaySE(AudioManager.SE.DiscardCard);

            // 카드 버림 루틴 시작 (카드를 버리는 애니메이션)
            StartCoroutine(DiscardRoutine(card.transform));
        }

        public void DiscardCard(CardView cardView)
        {
            var card = cards.Find(t => t == cardView);

            // 카드를 리스트에서 지움
            cards.Remove(card);
            // 버린 카드더미에 추가
            //battleManager.AddToDiscardPile(card);

            // 효과음 재생
            //AudioManager.PlaySE(AudioManager.SE.DiscardCard);

            // 카드 버림 루틴 시작 (카드를 버리는 애니메이션)
            StartCoroutine(DiscardRoutine(card.transform));
        }

        public void DiscardAllCards()
        {
            StartCoroutine(DiscardAllRoutine());
        }

        private IEnumerator DiscardRoutine(Transform card)
        {
            while (true)
            {
                // lerp 정도
                var lerpAmount = lerpTime * Time.deltaTime;

                // 타겟 위치를 설정
                var targetPos = discardPosition.localPosition;
                // 타겟 회전을 버린 카드더미를 바라보는 방향으로 설정
                var targetRot = Quaternion.FromToRotation(Vector3.up, (targetPos - card.localPosition).normalized);
                // 타겟 스케일을 설정
                var targetScl = discardPosition.localScale;

                // Lerp로 최종 위치, 회전, 스케일 설정
                card.localPosition = Vector3.Lerp(card.localPosition, targetPos, lerpAmount);
                card.localRotation = Quaternion.Lerp(card.localRotation, targetRot, lerpAmount * 1.5f);
                card.localScale = Vector3.Lerp(card.localScale, targetScl, lerpAmount * 0.5f);

                // 카드가 버린 카드더미에 도달하면
                if (Vector3.Distance(card.localPosition, targetPos) <= 1f)
                {
                    // 카드를 비활성화
                    //card.gameObject.SetActive(false);

                    // 루틴 종료
                    yield break;
                }

                // 1프레임 대기
                yield return null;
            }
        }

        private IEnumerator DiscardAllRoutine()
        {
            // 전체 카드를 역으로 순회하면서,
            for (var i = cards.Count - 1; i >= 0; i--)
            {
                // 카드를 버림
                DiscardCard(i);

                // 0.1초 대기
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
