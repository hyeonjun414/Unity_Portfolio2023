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
        public List<Transform> enemyPosList;
        public Transform heroPosition;
        public Transform doorPosition;
        public Transform cardPosition;
        public Transform handPos, deckPos, gravePos;
            

        public EntityView HeroView;
        public List<EnemyView> EnemyViews;
        public List<EnemyView> EnemyPrefabs;
        public List<CardView> UserCards = new();
        public List<CardView> DeckCards = new();
        public List<CardView> GraveCards = new();
        public List<CardView> HandCards = new();
        public GameObject indicator;

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
            await UniTask.Delay(1000);
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
            var enemyView = EnemyPrefabs.First(target => target.name == enemy.Name);
            var inst = Instantiate(enemyView, enemyPosList[index]);
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

            return doorInst;
        }

        public async UniTask MoveStage()
        {
            HeroView.transform.DOMove(doorPosition.position, 2f)
                .OnStart(() => HeroView.animator.SetBool("Move", true))
                .OnComplete(() => HeroView.animator.SetBool("Move", false));
            await UniTask.Delay(2000);
            HeroView.animator.SetTrigger("DoorIn");
            await UniTask.Yield();
            var clipLength = HeroView.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
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

            cardView.transform.DOScale(1, 0.2f);
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
                cv.transform.DOMove(handPos.position + (Vector3.right * (-cardStartPos + index * 100)), 0.5f);
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
                cardView.transform.DOMove(deckPos.position, 0.2f);
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
            cardView.transform.DOMove(gravePos.position, 0.5f);
            cardView.transform.SetParent(gravePos);

            await ReplaceHandCard();
        }

        
    }
}
