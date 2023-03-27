using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class Stage
    {
        public StageModel Model;
        public StageView View;

        public List<Enemy> Enemies = new();

        public bool IsAction;
        

        private GameManager gm;
        private User user;
        private Enemy _curTarget;
        private BattleCard _selectedCard;
        private Reward _reward;

        public List<BattleCard> Hand = new();
        public List<BattleCard> Deck = new();
        public List<BattleCard> Grave = new();
        private bool hasMovedToNextStage;
        private bool rewardGiven;

        public Stage(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            gm = GameManager.Instance;
            user = gm.User;
        }

        public void Init()
        {
            user.UserHero.View = View.CreateHeroView(gm.User.GetHeroModel());
            
            var enemyModels = Model.GetEnemies();
            for (var index = 0; index < enemyModels.Count; index++)
            {
                var enemyView = View.CreateEnemyView(index, enemyModels[index]);
                var enemyPresenter = new Enemy(enemyModels[index], enemyView);
                enemyPresenter.SetAction();
                Enemies.Add(enemyPresenter);
            }
            
            Deck = new List<BattleCard>(gm.User.GetCards());
            View.SetUserCards(Deck);
        }

        public async UniTask Update()
        {
            if (IsAction) return;
            
            UpdateActionGaugePhase();
            await ActionPhase();
        }

        private async UniTask CheckEnemies()
        {
            if (!Model.AreAllEnemiesDead()) return;

            GenerateReward();
            await View.BattleEnd();
            GenerateDoor();
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
                cards.Add(card);
            }
            _reward = new Reward(data, null);
            _reward.Init(cards);
            View.GenerateReward(_reward);
        }

        private void GenerateDoor()
        {
            var masterStage = gm.MasterTable.MasterStages[0];
            var doorModel = new DoorModel(masterStage);
            var doorView = View.GenerateDoor();
            var door = new Door(doorModel, doorView);
        }

        private async UniTask ActionPhase()
        {
            foreach (var enemy in GetAliveEnemies())
            {
                if (enemy.IsActExecutable())
                {
                    IsAction = true;
                    await enemy.ExecuteAction(user.UserHero);
                    IsAction = false;
                }
            }
        }

        private List<Enemy> GetAliveEnemies()
        {
            return Enemies.Where(enemy => !enemy.Model.IsDead).ToList();
        }

        private async UniTask UserActionReady()
        {
            IsAction = true;
            await DrawCard();

        }

        public async UniTask DrawCard()
        {
            if (user.UserHero.CanDrawCard())
            {
                user.UserHero.UseActionCount(1);
                var drawCount = user.GetDrawCount();
                if (Deck.Count == 0)
                {
                    await GraveToDeck();
                }

                while (drawCount > 0 && Deck.Count > 0)
                {
                    await DeckToHand(Deck[0]);
                    await UniTask.Delay(200);
                    drawCount--;
                }
            }
            
        }

        private async UniTask DeckToHand(BattleCard card)
        {
            Hand.Add(card);
            Deck.Remove(card);
            await View.DeckToHand(card);
        }

        private async UniTask GraveToDeck()
        {
            Deck.AddRange(Grave);
            Grave.Clear();
            await View.GraveToDeck(Deck);
        }

        private async UniTask HandToGrave(BattleCard card)
        {
            Grave.Add(card);
            Hand.Remove(card);
            await View.HandToGrave(card);
        }


        private async UniTask CardAttack(Enemy enemy)
        {
            IsAction = true;
            if (user.UserHero.GetActionCount() >= _selectedCard.GetCost())
            {
                UnTargetEnemy(enemy);
                await user.UseCard(_selectedCard, enemy);
                if (enemy.Model.IsDead)
                {
                    await CheckEnemies();
                }

                await HandToGrave(_selectedCard);
            }
            
            IsAction = false;
        }

        private void UpdateActionGaugePhase()
        {
            user.UserHero.AddActionGauge();

            foreach (var enemy in GetAliveEnemies())
                enemy.AddActionGauge();
        }

        public async UniTask MoveStage(Door door)
        {
            if (hasMovedToNextStage) return;
            hasMovedToNextStage = true;
            door.View.Open();
            await View.MoveStage();
            door.View.Close();
            await UniTask.Delay(500);
            await GameManager.Instance.LoadStageScene(door.GetStageData());
        }

        public void TargetEnemy(Enemy ep)
        {
            if (IsAction) return;
            
            _curTarget = ep;
            View.SetTargetIndicator(ep);
            
        }

        public void UnTargetEnemy(Enemy ep)
        {
            if (_curTarget == ep)
            {
                View.UnsetTargetIndicator(); 
                _curTarget = null;
            }
            
        }

        public void SelectCard(BattleCard card)
        {
            if (IsAction) return;
            
            _selectedCard = card;
            card.Selected();
        }

        public void UnSelectCard(BattleCard card)
        {
            if (_selectedCard == card)
            {
                if (_curTarget != null && !IsAction && user.CanUseThisCard(_selectedCard))
                {
                    CardAttack(_curTarget);
                }
                else
                {
                    View.UnsetTargetIndicator(); 
                    _selectedCard.UnSelected();
                    _selectedCard = null; 
                }

                
            }
        }

        public void CreateFloatingText(string str, Vector3 position)
        {
            View.CreateFloatingText(str, position);
        }


        public async UniTask OpenReward(ChestView chest)
        {
            if (rewardGiven) return;

            await chest.Open();
            await OpenRewardPanel();
        }

        public async UniTask CloseReward(BattleCard card)
        {
            if (card != null)
            {
                user.AddCard(card);
                rewardGiven = true;
            }

            
            await CloseRewardPanel();
        }

        private async UniTask CloseRewardPanel()
        {
            View.CloseRewardPanel();
        }

        private async UniTask OpenRewardPanel()
        {
            View.OpenRewardPanel();
        }
    }
}
