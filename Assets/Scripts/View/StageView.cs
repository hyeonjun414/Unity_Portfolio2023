using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Model;
using Presenter;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View
{
    public interface IStageView
    {
        EntityView CreateHeroView(EntityModel hero);
        EnemyView CreateEnemyView(int index, EnemyModel enemy);
    }
    
    public class StageView : MonoBehaviour, IStageView
    {
        public Stage Presenter;

        public EntityView entityView;
        public DoorView doorPrefab;
        public CardView cardPrefab;
        public EnemyView enemyPrefab;
        public List<Transform> enemyPosList;
        public Transform heroPosition;
        public Transform doorPosition;
        public Transform cardPosition;
        public Transform handPos, deckPos, gravePos;
            
        
        public EntityView HeroView;
        public List<EnemyView> EnemyViews;
        public List<CardView> UserCards = new();
        public List<CardView> DeckCards = new();
        public List<CardView> GraveCards = new();
        public List<CardView> HandCards = new();
        public GameObject indicator;
        public FloatingTextView floatingText;
        public RewardView rewardView;
        public ChestView chestPrefab;

        private bool _isBattleEnd;
        
        public void Start()
        {
            if (GameManager.Instance == null) 
                return;

            EnemyViews = new List<EnemyView>();
            
            Presenter = GameManager.Instance.CurStage;
            Presenter.View = this;
            Presenter.Init();
        }

        private async void Update()
        {
            if (_isBattleEnd) return;
            
            await Presenter.Update();
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
        
        public EnemyView CreateEnemyView(int index, EnemyModel enemy)
        {
            var inst = Instantiate(enemyPrefab, enemyPosList[index]);
            var enemyPresenter = new Enemy(enemy, inst);
            enemyPresenter.Init();
            
            EnemyViews.Add(inst);
            return inst;
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

            cardView.transform.DOScale(1, 0.2f).SetEase(Ease.OutExpo);
            cardView.transform.DORotate(Vector3.zero, 0.2f);
            cardView.transform.SetParent(handPos);

            await ReplaceHandCard();
        }

        public async UniTask ReplaceHandCard()
        {
            var cardStartPos = (HandCards.Count -1)*100 * 0.5f;
            for (var index = 0; index < HandCards.Count; index++)
            {
                var cv = HandCards[index];
                cv.transform.DOMove(handPos.position + (Vector3.right * (-cardStartPos + index * 100)), 0.2f)
                    .SetEase(Ease.OutExpo);
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
                cardView.transform.DOMove(deckPos.position, 0.2f).SetEase(Ease.OutExpo);
                cardView.transform.DORotate(new Vector3(0, 180, 0), 0.2f);

                await UniTask.Delay(200);
            }
        }

        public async UniTask HandToGrave(Card card)
        {
            var cardView = card.View;
            if (cardView == null) return;

            HandCards.Remove(cardView);
            GraveCards.Add(cardView);

            cardView.transform.DOScale(0.5f, 0.2f);
            cardView.transform.DORotate(new Vector3(0, 180, Random.Range(-20, 20)), 0.2f);
            cardView.transform.DOMove(gravePos.position, 0.2f).SetEase(Ease.OutExpo);
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
            Instantiate(chestPrefab);
            rewardView.Init(reward);
        }
    }
}
