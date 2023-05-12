using System;
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
using Random = UnityEngine.Random;

namespace View.StageView
{
    public class BattleStageView : StageView
    {
        private BattleStage bsPresenter => Presenter as BattleStage;


        public TextMeshProUGUI energyText;
        public CharacterView characterView;
        public AllyView allyView;
        public DoorView doorPrefab;
        public CardView cardPrefab;
        public EnemyView enemyPrefab;
        public ChestView chestPrefab;
        public ArtifactView artifactPrefab;
        public Transform deckPos;
        public Transform DoorPivot, chestPivot, artifactPivot;
            
        public List<EnemyView> EnemyViews;
        public List<CardView> UserCards = new();
        public List<CardView> DeckCards = new();
        public List<CardView> GraveCards = new();
        public FloatingTextView floatingText;
        public RewardView rewardView;
        public GameOverView gameOverView;

        public Button turnEndButton;
        public ActionBar actionBar;

        public CardHolder cardHolder;
        public CharacterHolder characterHolder;
        public UserGoldView userGoldView;
        
        private CharacterView _heroView;
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
            await UniTask.Delay(500);
            foreach (var ev in EnemyViews)
            {
                ev.Presenter.Dispose();
            }
        }

        public void CreateHeroView(Character hero)
        {
            var inst = Instantiate(characterView);
            hero.View = inst;
            _heroView = hero.View;
            _heroView.Init(hero);
            _heroView.gameObject.SetActive(true);
            characterHolder.AddCharacterView(inst);
            actionBar.AddEntity(hero);
        }

        public void SummonAlly(Ally ally)
        {
            var inst = Instantiate(allyView);
            inst.Init(ally);
            inst.gameObject.SetActive(true);
            characterHolder.AddCharacterView(inst);
            actionBar.AddEntity(ally);
        }

        public DoorView GenerateDoor()
        {
            var doorInst = Instantiate(doorPrefab, DoorPivot);
            doorInst.transform.localPosition = Vector3.down * 5;
            doorInst.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutExpo);
            return doorInst;
        }

        public async UniTask MoveStage()
        {
            _heroView.content.transform.DOMove(DoorPivot.position, 1f)
                .OnStart(() => _heroView.animator.SetBool("Move", true))
                .OnComplete(() => _heroView.animator.SetBool("Move", false));
            await UniTask.Delay(1000);
            _heroView.animator.SetTrigger("DoorIn");
            await UniTask.Yield();
            var clipLength = _heroView.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / _heroView.animator.speed;
            _heroView.content.transform.DOScale(0.8f, clipLength)
                .OnComplete(() => _heroView.gameObject.SetActive(false));
            
            await UniTask.Delay((int)(clipLength * 1000));
            
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
            cardHolder.DrawCard(cardView);
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

            cardHolder.DiscardCard(cardView);
            GraveCards.Add(cardView);
        }


        public void CreateFloatingText(string str, Vector3 position, TextType textType)
        {
            var textInst = Instantiate(floatingText);
            textInst.SetFloatingText(str, position, textType);
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
            Instantiate(chestPrefab, chestPivot);
            rewardView.Init(reward);
        }

        public void SetEnemyViews(List<Character> enemies)
        {
            var xGap = 3f;
            var mostLeft = -(enemies.Count - 1) * 0.5f * xGap;
            for (var i = 0; i < enemies.Count; i++)
            {
                var inst = Instantiate(enemyPrefab);
                inst.transform.localPosition += Vector3.right * (mostLeft + i * xGap);
                enemies[i].View = inst;
                inst.Presenter = enemies[i];
                EnemyViews.Add(inst);
                actionBar.AddEntity(enemies[i]);
                characterHolder.AddCharacterView(inst);
            }
        }

        public void SetEnergyText(int userCurEnergy, int userMaxEnergy)
        {
            energyText.SetText($"{userCurEnergy} / {userMaxEnergy}");
        }

        public void TurnEnded()
        {
            turnEndButton.interactable = false;
        }

        public void TurnStarted()
        {
            turnEndButton.interactable = true;
        }

        public void CardHovered(CardView cardView)
        {
            cardHolder.CardHovered(cardView);
        }
        public void CardUnHovered(CardView cardView)
        {
            cardHolder.CardUnHovered(cardView);
        }


        public void CardSelected(CardView cardView)
        {
            cardView.SetInputChecker(false);
            cardHolder.CardSelected(cardView);
        }

        public void CardUnSelected(CardView cardView)
        {
            cardView.SetInputChecker(true);
            cardHolder.CardUnSelected();
        }

        public async UniTask EntityRemoved(Character character)
        {
            await characterHolder.RemoveCharacterView(character.View);
        }


        public void PositionSwitched(Character character, int targetIndex, int moveIndex)
        {
            characterHolder.PositionSwitched(character, targetIndex, moveIndex);
            
        }

        public void GameOvered()
        {
            gameOverView.gameObject.SetActive(true);
        }

        public void SetUserArtifacts(List<Artifact> userArtifacts)
        {
            foreach (var artifact in userArtifacts)
            {
                var inst = Instantiate(artifactPrefab, artifactPivot);
                inst.SetView(artifact);
            }
        }

        public void SetUserGold(int userGold)
        {
            userGoldView.Init(userGold);
        }

        public void AddDropGold(int userGold, int dropGold)
        {
            CreateFloatingText(dropGold.ToString(), userGoldView.goldText.transform.position, TextType.Gold);
            userGoldView.AddGold(userGold, dropGold);
        }
    }
}
