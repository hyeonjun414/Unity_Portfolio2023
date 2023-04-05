using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;
using View.StageView;

namespace Presenter
{
    public class Stage
    {
        public StageModel Model;
        public StageView View;
        
        protected GameManager gm;
        protected User user;
        

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

        public virtual async UniTask Update()
        {
            await UniTask.Yield();
        }
    }

    public class BattleStage : Stage
    {
        private BattleStageModel bsModel => Model as BattleStageModel;
        private BattleStageView bsView => View as BattleStageView;

        private Enemy _curTarget;
        private BattleCard _selectedCard;
        private Reward _reward;
        public bool IsAction;
        public List<Enemy> Enemies = new();
        public List<BattleCard> Hand = new();
        public List<BattleCard> Deck = new();
        public List<BattleCard> Grave = new();
        private bool hasMovedToNextStage;
        private bool rewardGiven;
        
        public BattleStage(StageModel model, StageView view) : base(model, view)
        {
        }

        public override void Init()
        {
            base.Init();
            
            user.UserHero.View = bsView.CreateHeroView(gm.User.GetHeroModel());
            var enemyModels = ((BattleStageModel)Model).GetEnemies();
            for (var index = 0; index < enemyModels.Count; index++)
            {
                var enemyView = bsView.CreateEnemyView(index);
                var enemyPresenter = new Enemy(enemyModels[index], enemyView);
                enemyPresenter.Init();
                Enemies.Add(enemyPresenter);
            }

            var userCardData = gm.User.GetCards();
            foreach (var cardData in userCardData)
            {
                Deck.Add(new BattleCard(cardData, null));
            }
            bsView.SetUserCards(Deck);
        }

        public override async UniTask Update()
        {
            if (IsAction) return;

            await UpdateActionGaugePhase();
            await StatusEffectPhase();
            await ActionPhase();
        }

        private async UniTask StatusEffectPhase()
        {
            await user.UserHero.StatusEffectActivate();
            foreach (var enemy in GetAliveEnemies())
            {
                IsAction = true;
                await enemy.StatusEffectActivate();
                IsAction = false;
            }
        }

        private async UniTask CheckEnemies()
        {
            if (Model is BattleStageModel sn && !sn.AreAllEnemiesDead()) return;

            GenerateReward();
            await bsView.BattleEnd();
            GenerateDoor();
        }

        private void GenerateReward()
        {
            var data = new RewardModel();
            var cards = new List<RewardCard>();
            var cardTable = gm.MasterTable.MasterCards;
            for (var i = 0; i < 3; i++)
            {
                var cardModel = new CardModel(cardTable[Random.Range(0, cardTable.Count)]);
                var card = new RewardCard(cardModel, null);
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
            await bsView.DeckToHand(card);
        }

        private async UniTask GraveToDeck()
        {
            Deck.AddRange(Grave);
            Grave.Clear();
            await bsView.GraveToDeck(Deck);
        }

        private async UniTask HandToGrave(BattleCard card)
        {
            Grave.Add(card);
            Hand.Remove(card);
            await bsView.HandToGrave(card);
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

        private async UniTask UpdateActionGaugePhase()
        {
            await user.UserHero.AddActionGauge();

            foreach (var enemy in GetAliveEnemies())
                await enemy.AddActionGauge();
        }

        public async UniTask MoveStage(Door door)
        {
            if (hasMovedToNextStage) return;
            hasMovedToNextStage = true;
            door.View.Open();
            await bsView.MoveStage();
            door.View.Close();
            await UniTask.Delay(500);
            //await GameManager.Instance.LoadStageScene(door.GetStageData());
        }

        public void TargetEnemy(Enemy ep)
        {
            if (IsAction) return;
            
            _curTarget = ep;
            bsView.SetTargetIndicator(ep);
            
        }

        public void UnTargetEnemy(Enemy ep)
        {
            if (_curTarget == ep)
            {
                bsView.UnsetTargetIndicator(); 
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
                    bsView.UnsetTargetIndicator(); 
                    _selectedCard.UnSelected();
                    _selectedCard = null; 
                }

                
            }
        }

        public void CreateFloatingText(string str, Vector3 position)
        {
            bsView.CreateFloatingText(str, position);
        }


        public async UniTask OpenReward(ChestView chest)
        {
            if (rewardGiven) return;

            await chest.Open();
            await OpenRewardPanel();
        }

        public async UniTask CloseReward(Card card)
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
            bsView.CloseRewardPanel();
        }

        private async UniTask OpenRewardPanel()
        {
            bsView.OpenRewardPanel();
        }
    }
}
