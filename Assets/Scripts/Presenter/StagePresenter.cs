using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class StagePresenter
    {
        public StageModel Model;
        public StageView View;

        public EntityPresenter HeroPresenter;
        public List<EnemyPresenter> EnemyPresenters = new();

        public List<DoorPresenter> Doors = new();
        
        public bool IsAction;
        

        private GameManager gm;
        private UserPresenter user;
        private EnemyPresenter _curTarget;
        private CardPresenter _selectedCard;

        public List<CardPresenter> Hand = new();
        public List<CardPresenter> Deck = new();
        public List<CardPresenter> Grave = new();
        
        public StagePresenter(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            gm = GameManager.Instance;
            user = gm.User;
        }

        public void Init()
        {
            View.CreateHeroView(gm.User.GetHero());
            var heroView = View.GetHeroView();
            HeroPresenter = user.HeroPresenter;
            heroView.Presenter = HeroPresenter;
            HeroPresenter.View = heroView;

            var enemyModels = Model.GetEnemies();
            for (var index = 0; index < enemyModels.Count; index++)
            {
                View.CreateEnemyView(index, enemyModels[index]);
            }

            var enemyViews = View.GetEnemyViews();
            for (var index = 0; index < enemyViews.Count; index++)
            {
                var enemyPresenter = new EnemyPresenter(enemyModels[index], enemyViews[index]);
                enemyViews[index].Presenter = enemyPresenter;
                EnemyPresenters.Add(enemyPresenter);
            }

            Deck = new List<CardPresenter>(gm.User.GetCards());
            View.SetUserCards(Deck);
        }

        public async UniTask Update()
        {
            if (IsAction) return;
            
            UpdateActionGaugePhase();
            await ActionPhase();
        }

        private void CheckEnemies()
        {
            var allEnemyDead = EnemyPresenters.All(target => target.Model.IsDead);
            if (allEnemyDead)
            {
                foreach (var enemy in EnemyPresenters)
                {
                    enemy.Dispose();
                }

                EnemyPresenters.Clear();
                View.BattleEnd();

                GenerateDoor();
            }
        }

        private void GenerateDoor()
        {
            var masterStage = gm.MasterTable.MasterStages[0];
            var doorModel = new DoorModel(masterStage);
            var doorPresenter = new DoorPresenter(doorModel, View.GenerateDoor());
            
            Doors.Add(doorPresenter);
        }

        private async UniTask ActionPhase()
        {
            if (HeroPresenter.Model.IsActionReady)
            {
                await UserActionReady();
            }

            foreach (var enemy in EnemyPresenters)
            {
                if (enemy.Model.IsActionReady && !enemy.Model.IsDead)
                    await Attack(enemy, HeroPresenter);
            }
        }

        private async UniTask UserActionReady()
        {
            IsAction = true;
            await DrawCard();

        }

        private async UniTask DrawCard()
        {
            var drawCount = user.Model.DrawCardCount;
            if (Deck.Count == 0)
            {
                await GraveToDeck();
            }

            while(drawCount > 0 && Deck.Count > 0)
            {
                await DeckToHand();
                await UniTask.Delay(200);
                drawCount--;
            }
        }

        private async UniTask DeckToHand()
        {
            var card = Deck[0];
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

        private async UniTask HandToGrave(CardPresenter card)
        {
            Grave.Add(card);
            Hand.Remove(card);
            await View.HandToGrave(card);
        }


        private async UniTask CardAttack(EntityPresenter target)
        {
            IsAction = true;
            await user.UseCard(_selectedCard, _curTarget);
            if (target.Model.IsDead)
            {
                if (target == _curTarget)
                {
                    _curTarget = null;
                    View.UnsetTargetIndicator();
                }

                CheckEnemies();
            }
            IsAction = false;
        }
        private async UniTask Attack(EntityPresenter atker, EntityPresenter target)
        {
            IsAction = true;
            await atker.PrepareAttack(target.View.GetPosition());
            await atker.PlayAttack();
            await target.TakeDamage(atker.Model.Damage);
            if (target.Model.IsDead)
            {
                if (target == _curTarget)
                {
                    _curTarget = null;
                    View.UnsetTargetIndicator();
                }
                CheckEnemies();
            }
                
            
            await atker.EndAttack(target.View.GetPosition());
            IsAction = false;
        }

        private void UpdateActionGaugePhase()
        {
            HeroPresenter.AddActionGauge();

            foreach (var enemy in EnemyPresenters.Where(target => !target.Model.IsDead))
                enemy.AddActionGauge();
        }

        public async UniTask MoveStage()
        {
            await View.MoveStage();
        }

        public void TargetEnemy(EnemyPresenter ep)
        {
            _curTarget = ep;
            View.SetTargetIndicator(ep);
        }

        public void UnTargetEnemy(EnemyPresenter ep)
        {
            if (_curTarget == ep)
            {
                View.UnsetTargetIndicator(); 
                _curTarget = null;
            }
            
        }

        public void SelectCard(CardPresenter card)
        {
            if (HeroPresenter.Model.IsActionReady)
            {
                _selectedCard = card;
                card.Selected();
            }
        }

        public void UnSelectCard(CardPresenter card)
        {
            if (_selectedCard == card)
            {
                if (_curTarget != null)
                {
                    CardAttack(_curTarget);
                    HandToGrave(_selectedCard);
                    UnTargetEnemy(_curTarget);
                }
                else
                {
                    _selectedCard.UnSelected();
                }

                _selectedCard = null; 
            }
        }

        
    }
}
