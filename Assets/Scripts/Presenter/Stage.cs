using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;
using View.StageView;
using Random = UnityEngine.Random;

namespace Presenter
{
    public class Stage
    {
        public StageModel Model;
        public StageView View;
        
        protected GameManager gm;
        protected User user;

        private readonly Queue<UniTask> _queue = new Queue<UniTask>();
        protected bool IsAction;
        public Stage(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            gm = GameManager.Instance;
            user = gm.User;
        }

        public virtual void Init()
        {
        }

        // public void Enqueue(UniTask action)
        // {
        //     Debug.Log("ViewAction Added");
        //     _queue.Enqueue(action);
        // }
        //
        // public async UniTask ExecuteAllAction()
        // {
        //     while (true)
        //     {
        //         await UniTask.Yield();
        //         if (_queue.Count == 0) continue;
        //         await UniTask.Delay(1000);
        //         var action = _queue.Dequeue();
        //         Debug.Log("ViewAction Execute");
        //         await action;
        //     }
        // }

        public virtual async UniTask Update()
        {
            await UniTask.Yield();
        }

        public virtual async UniTask StageClear()
        {
            await UniTask.Yield();
            await GameManager.Instance.LoadMapScene();
        }


        
    }

    public class BattleStage : Stage
    {
        private BattleStageModel bsModel => Model as BattleStageModel;
        private BattleStageView bsView => View as BattleStageView;

        private Character _curTarget;
        private Card _selectedCard;
        private Reward _reward;
        public List<Enemy> Enemies = new();
        public List<Card> Hand = new();
        public List<Card> Deck = new();
        public List<Card> Grave = new();
        private bool hasMovedToNextStage;
        private bool rewardGiven;

        private bool _isHeroTurn;
        private bool _isStageClear;
        private bool _inCardZone;

        public BattleStage(StageModel model, StageView view) : base(model, view)
        {
        }

        public override void Init()
        {
            base.Init();
            
            bsView.CreateHeroView(gm.User.UserHero);
            bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
            gm.User.UserHero.hModel.UseAp();
            var enemyModels = bsModel.GetEnemies();
            for (var index = 0; index < enemyModels.Count; index++)
            {
                var enemyPresenter = new Enemy(enemyModels[index], null);
                enemyPresenter.OnDeath += OnDeath;
                Enemies.Add(enemyPresenter);
            }

            bsView.SetEnemyViews(Enemies);
            foreach (var enemy in Enemies)
            {
                enemy.Init();
            }

            var userCardData = gm.User.GetCards();
            foreach (var cardData in userCardData)
            {
                var card = new Card(cardData, null);
                card.SetState(new CardBattleState());
                Deck.Add(card);
            }
            bsView.SetUserCards(Deck);
        }

        public async void OnDeath(object sender, EventArgs e)
        {
            if (sender is Hero)
            {
                GameOver();
            }
            else if (sender is Enemy enemy)
            {
                await RemoveEntityView(enemy);
                await CheckEnemies();
            }
        }

        private async UniTask RemoveEntityView(Character character)
        {
            await bsView.EntityRemoved(character);
        }

        private void GameOver()
        {
            throw new NotImplementedException();
        }

        public void StageStart()
        {
            var Modeltask = Update();
        }

        public override async UniTask Update()
        {
            while (!_isStageClear)
            {
                await UniTask.Yield();
                if (_isHeroTurn)
                {
                    continue;
                }
                else
                {
                    await AddEntityAp();
                }

                
            }
        }

        private async UniTask AddEntityAp()
        {
            var deltaTime = Time.deltaTime * 5;
            user.UserHero.AddAp(deltaTime);
            if (user.UserHero.Model.IsReady)
            {
                _isHeroTurn = true;
                user.SetEnergy();
                bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
                bsView.TurnStarted();
                await DrawCard(user.GetDrawCount());
                return;
            }

            foreach (var enemy in GetAliveEnemies())
            {
                enemy.AddAp(deltaTime);
                if (enemy.Model.IsReady)
                {
                    await enemy.StatusEffectActivate();
                    if (enemy.Model.IsDead)
                        continue;
                    await enemy.ExecuteAction(user.UserHero);
                }
            }

            await UniTask.Yield();

        }
        

        private async UniTask CheckEnemies()
        {
            if (Model is BattleStageModel sn && !sn.AreAllEnemiesDead()) return;

            GenerateReward();
            await BattleEnd();
            GenerateDoor();
        }

        private async UniTask BattleEnd()
        {
            user.UserHero.OnDeath -= OnDeath;
            await bsView.BattleEnd();
        }

        private void GenerateReward()
        {
            var data = new RewardModel();
            var cards = new List<Card>();
            var cardTable = gm.MasterTable.MasterCards;
            for (var i = 0; i < 3; i++)
            {
                var cardModel = new CardModel(cardTable[Random.Range(0, cardTable.Count)]);
                var card = new Card(cardModel, null);
                card.SetState(new CardRewardState());
                cards.Add(card);
            }
            _reward = new Reward(data, null);
            _reward.Init(cards);
            bsView.GenerateReward(_reward);
        }

        private void GenerateDoor()
        {
            var masterStage = gm.MasterTable.MasterStages[0];
            var doorModel = new DoorModel(masterStage);
            var doorView = bsView.GenerateDoor();
            var door = new Door(doorModel, doorView);
        }

        private async UniTask ActionPhase()
        {
            foreach (var enemy in GetAliveEnemies())
            {
                await enemy.ExecuteAction(user.UserHero);
            }
        }

        private List<Enemy> GetAliveEnemies()
        {
            return Enemies.Where(enemy => !enemy.Model.IsDead).ToList();
        }

        public async UniTask DrawCard(int drawCount)
        {
            while (drawCount > 0)
            {
                if (Deck.Count > 0)
                {
                    await DeckToHand(Deck[0]);
                    await UniTask.Delay(50);
                    drawCount--;
                }
                else if (Grave.Count > 0)
                {
                    await GraveToDeck();
                }
            }
        }

        private async UniTask DeckToHand(Card card)
        {
            Hand.Add(card);
            Deck.Remove(card);
            await bsView.DeckToHand(card);
        }

        private async UniTask GraveToDeck()
        {
            Deck.AddRange(Grave);
            Grave.Clear();
            await bsView.GraveToDeck(Deck);
        }

        private async UniTask HandToGrave(Card card)
        {
            Grave.Add(card);
            Hand.Remove(card);
            await bsView.HandToGrave(card);
        }


        private async UniTask UseCard()
        {
            bsView.CardUnSelected(_selectedCard.View);
            
            switch (_selectedCard.GetCardType())
            {
                case CardType.Attack:
                    if (_curTarget == null) break;
                    await user.UseCard(_selectedCard, _curTarget);
                    await HandToGrave(_selectedCard);
                    
                    break;
                case CardType.Magic:
                    if (_inCardZone)
                    {
                        await user.UseCard(_selectedCard, user.UserHero);
                        await HandToGrave(_selectedCard);
                    }
                    break;
            }

            bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
            UnTargetEntity();
            _selectedCard = null;
        }

        public async UniTask MoveStage(Door door)
        {
            if (hasMovedToNextStage) return;
            hasMovedToNextStage = true;
            door.View.Open();
            await bsView.MoveStage();
            door.View.Close();
            await UniTask.Delay(500);
            await base.StageClear();
        }

        public void TargetEntity(Character character)
        {
            if ((_selectedCard.GetCardType() is CardType.Attack && character is Enemy) ||
                (_selectedCard.GetCardType() is CardType.Magic && character is Hero))
            {
                _curTarget = character;
                bsView.SetTargetIndicator(character.View);
            }
        }

        public void UnTargetEntity()
        {
            bsView.UnsetTargetIndicator();
            _curTarget = null;
        }

        public void HoverCard(Card card)
        {
            if (_selectedCard != null) return; 
            bsView.CardHovered(card.View);
        }

        public void UnHoverCard(Card card)
        {
            if (_selectedCard != null) return; 
            bsView.CardUnHovered(card.View);
        }
        
        public void SelectCard(Card card)
        {
            Debug.Log("Card click");
            _selectedCard = card;
            bsView.CardSelected(card.View);
        }

        public void UnSelectCard(Card card)
        {
            if (user.CanUseThisCard(_selectedCard))
            {
                var task = UseCard();
            }
            else
            {
                bsView.UnsetTargetIndicator();
                bsView.CardUnSelected(_selectedCard.View);
                _selectedCard = null;
            }
        }

        public void CreateFloatingText(string str, Vector3 position, TextType textType)
        {
            bsView.CreateFloatingText(str, position, textType);
        }


        public async UniTask OpenReward(ChestView chest)
        {
            if (rewardGiven) return;

            await chest.Open();
            OpenRewardPanel();
        }

        public async UniTask CloseReward(Card card)
        {
            if (card != null)
            {
                user.AddCard(card);
                rewardGiven = true;
            }

            CloseRewardPanel();

            await UniTask.Yield();
        }

        private void CloseRewardPanel()
        {
            bsView.CloseRewardPanel();
        }

        private void OpenRewardPanel()
        {
            bsView.OpenRewardPanel();
        }

        public async UniTask TurnEnd()
        {
            for (int i = Hand.Count-1; i >= 0; i--)
            {
                await HandToGrave(Hand[i]);
            }

            _isHeroTurn = false;
            user.UserHero.hModel.UseAp();
            bsView.TurnEnded();
        }


        public void EnterCardZone()
        {
            if (_selectedCard == null) return;
            _inCardZone = true;
        }

        public void ExitCardZone()
        {
            _inCardZone = false;
        }
    }
}
