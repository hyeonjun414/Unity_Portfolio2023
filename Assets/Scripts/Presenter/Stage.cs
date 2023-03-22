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
        private Card _selectedCard;

        public List<Card> Hand = new();
        public List<Card> Deck = new();
        public List<Card> Grave = new();
        private bool hasMovedToNextStage;

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
            
            Deck = new List<Card>(gm.User.GetCards());
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
            
            await View.BattleEnd();
            GenerateDoor();
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

        private async UniTask DeckToHand(Card card)
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

        private async UniTask HandToGrave(Card card)
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
                await user.UseCard(_selectedCard, enemy);
                UnTargetEnemy(enemy);
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
            await UniTask.Delay(1000);
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

        public void SelectCard(Card card)
        {
            if (IsAction) return;
            
            _selectedCard = card;
            card.Selected();
        }

        public void UnSelectCard(Card card)
        {
            if (_selectedCard == card)
            {
                if (_curTarget != null)
                {
                    CardAttack(_curTarget);
                }
                else
                {
                    _selectedCard.UnSelected();
                    _selectedCard = null; 
                }

                
            }
        }

        
    }
}
