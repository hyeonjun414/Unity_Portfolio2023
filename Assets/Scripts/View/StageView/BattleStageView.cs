using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using Model;
using Presenter;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View.StageView
{
    public class BattleStageView : StageView
    {
        private BattleStage bsPresenter => Presenter as BattleStage;


        public TextMeshProUGUI energyText;
        public EntityView entityView;
        public DoorView doorPrefab;
        public CardView cardPrefab;
        public EnemyView enemyPrefab;
        public ChestView chestPrefab;
        public List<Transform> enemyPosList;
        public Transform heroPosition;
        public Transform doorPosition;
        public Transform handPos, deckPos, gravePos;
        public Transform cardForwardPivot;
            
        public List<EnemyView> EnemyViews;
        public List<CardView> UserCards = new();
        public List<CardView> DeckCards = new();
        public List<CardView> GraveCards = new();
        public List<CardView> HandCards = new();
        public GameObject indicator;
        public FloatingTextView floatingText;
        public RewardView rewardView;

        public Button turnEndButton; 
        
        
        private EntityView HeroView;
        private bool _isBattleEnd;
        private CardView _hoveredCard;
        
        public void Start()
        {
            if (GameManager.Instance == null) 
                return;

            EnemyViews = new List<EnemyView>();
            
            Presenter = GameManager.Instance.CurStage;
            Presenter.View = this;
            Presenter.Init();

            turnEndButton.onClick.AsObservable().Subscribe(async _ =>
            {
                await bsPresenter.TurnEnd();
            });
        }

        public async UniTask BattleEnd()
        {
            _isBattleEnd = true;
            await UniTask.Delay(500);
            foreach (var ev in EnemyViews)
            {
                ev.Presenter.Dispose();
            }
        }

        public EntityView CreateHeroView(EntityModel hero)
        {
            HeroView = Instantiate(entityView, heroPosition);
            HeroView.Init(hero);
            HeroView.gameObject.SetActive(true);

            return HeroView;
        }
        public EntityView GetHeroView()
        {
            return HeroView;
        }

        public List<EnemyView> GetEnemyViews()
        {
            return EnemyViews;
        }

        public DoorView GenerateDoor()
        {
            var doorInst = Instantiate(doorPrefab, doorPosition);
            doorInst.transform.localPosition = Vector3.down * 5;
            doorInst.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutExpo);
            return doorInst;
        }

        public async UniTask MoveStage()
        {
            HeroView.transform.DOMove(doorPosition.position, 1f)
                .OnStart(() => HeroView.animator.SetBool("Move", true))
                .OnComplete(() => HeroView.animator.SetBool("Move", false));
            await UniTask.Delay(1000);
            HeroView.animator.SetTrigger("DoorIn");
            await UniTask.Yield();
            var clipLength = HeroView.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / HeroView.animator.speed;
            HeroView.transform.DOScale(0.8f, clipLength)
                .OnComplete(() => HeroView.gameObject.SetActive(false));
            
            await UniTask.Delay((int)(clipLength * 1000));
            
        }

        public void SetTargetIndicator(Enemy ep)
        {
            var targetView = EnemyViews.FirstOrDefault(target => target.Presenter == ep);
            if (targetView != null)
            {
                indicator.SetActive(true);
                indicator.transform.position = targetView.transform.position + Vector3.up * 2.5f;
            }
        }

        public void UnsetTargetIndicator()
        {
            indicator.SetActive(false);
        }

        public void SetUserCards(List<Card> Cards)
        {
            foreach (var card in Cards)
            {
                var cardView = Instantiate(cardPrefab, deckPos);
                cardView.SetView(card);
                cardView.transform.rotation = Quaternion.Euler(0, 180, 0);
                cardView.transform.DOScale(0.5f, 0.2f);
                card.View = cardView;
                DeckCards.Add(cardView);
            }
        }

        public async UniTask DeckToHand(Card card)
        {
            var cardView = card.View;
            if (cardView == null) return;
            
            DeckCards.Remove(cardView);
            HandCards.Add(cardView);

            cardView.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuad);
            cardView.transform.DORotate(Vector3.zero, 0.3f);
            cardView.transform.SetParent(handPos);

            await ReplaceHandCard();
        }

        public async UniTask ReplaceHandCard(CardView selectedCard = null)
        {
            var cardStartPos = (HandCards.Count -1)*100 * 0.5f;
            var selectedCardIndex = HandCards.IndexOf(selectedCard);
            for (var index = 0; index < HandCards.Count; index++)
            {
                var cv = HandCards[index];
                if (selectedCardIndex == -1)
                {
                    cv.transform.DOMove(handPos.position + (Vector3.right * (-cardStartPos + index * 100)), 0.3f)
                        .SetEase(Ease.OutQuad);
                }
                else
                {
                    cv.transform.DOMove(selectedCard.transform.position + (Vector3.right * ((index - selectedCardIndex) * 100)), 0.3f)
                        .SetEase(Ease.OutQuad);
                }
            }

            await UniTask.Yield();
        }

        public async UniTask GraveToDeck(List<Card> deck)
        {
            foreach (var card in deck)
            {
                var cardView = card.View;
                GraveCards.Remove(cardView);
                DeckCards.Add(cardView);
                cardView.transform.SetParent(deckPos);
                cardView.transform.DOMove(deckPos.position, 0.2f).SetEase(Ease.OutQuad);
                cardView.transform.DORotate(new Vector3(0, 180, 0), 0.3f);

                await UniTask.Delay(300);
            }
        }

        public async UniTask HandToGrave(Card card)
        {
            var cardView = card.View;
            if (cardView == null) return;

            HandCards.Remove(cardView);
            GraveCards.Add(cardView);

            cardView.transform.DOScale(0.5f, 0.2f);
            cardView.transform.DORotate(new Vector3(0, 180, Random.Range(-20, 20)), 0.3f);
            cardView.transform.DOMove(gravePos.position, 0.3f).SetEase(Ease.OutQuad);
            cardView.transform.SetParent(gravePos);

            await ReplaceHandCard();
        }


        public void CreateFloatingText(string str, Vector3 position)
        {
            var textInst = Instantiate(floatingText);
            textInst.SetFloatingText(str, position);
        }

        public void OpenRewardPanel()
        {
            rewardView.gameObject.SetActive(true);
        }

        public void CloseRewardPanel()
        {
            rewardView.gameObject.SetActive(false);
        }

        public void GenerateReward(Reward reward)
        {
            Instantiate(chestPrefab, transform);
            rewardView.Init(reward);
        }

        public void SetEnemyViews(List<Enemy> enemies)
        {
            for (var i = 0; i < enemies.Count; i++)
            {
                var inst = Instantiate(enemyPrefab, enemyPosList[i]);
                enemies[i].View = inst;
                inst.Presenter = enemies[i];
                EnemyViews.Add(inst);
                
            }
        }

        public void SetEnergyText(int userCurEnergy, int userMaxEnergy)
        {
            energyText.SetText($"{userCurEnergy} / {userMaxEnergy}");
        }
    }
}
