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
            HeroPresenter = user.HeroPresenter;
            HeroPresenter.View = View.CreateHeroView(gm.User.GetHero());
            
            var enemyModels = Model.GetEnemies();
            for (var index = 0; index < enemyModels.Count; index++)
            {
                var enemyView = View.CreateEnemyView(index, enemyModels[index]);
                var enemyPresenter = new EnemyPresenter(enemyModels[index], enemyView);
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
            var doorPresenter = new DoorPresenter(doorModel, doorView);
        }

        private async UniTask ActionPhase()
        {
            if (HeroPresenter.GetActionReady())
            {
                await UserActionReady();
            }

            foreach (var enemy in GetAliveEnemyPresenters())
            {
                if (enemy.GetActionReady())
                    await EnemyAttack(enemy);
            }
        }

        private List<EnemyPresenter> GetAliveEnemyPresenters()
        {
            return EnemyPresenters.Where(enemy => !enemy.Model.IsDead).ToList();
        }

        private async UniTask UserActionReady()
        {
            IsAction = true;
            await DrawCard();

        }

        private async UniTask DrawCard()
        {
            var drawCount = user.GetDrawCount();
            if (Deck.Count == 0)
            {
                await GraveToDeck();
            }

            while(drawCount > 0 && Deck.Count > 0)
            {
                await DeckToHand(Deck[0]);
                await UniTask.Delay(200);
                drawCount--;
            }
        }

        private async UniTask DeckToHand(CardPresenter card)
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

        private async UniTask HandToGrave(CardPresenter card)
        {
            Grave.Add(card);
            Hand.Remove(card);
            await View.HandToGrave(card);
        }


        private async UniTask CardAttack(EnemyPresenter enemy)
        {
            IsAction = true;
            await user.UseCard(_selectedCard, enemy);
            if (enemy.Model.IsDead)
            {
                UnTargetEnemy(enemy);
                await CheckEnemies();
            }

            await HandToGrave(_selectedCard);
            IsAction = false;
        }
        private async UniTask EnemyAttack(EntityPresenter atker)
        {
            IsAction = true;
            await atker.PrepareAttack(HeroPresenter.View.GetPosition());
            await atker.PlayAttack();
            await HeroPresenter.TakeDamage(atker.Model.Damage);
            await atker.EndAttack();
            IsAction = false;
        }

        private void UpdateActionGaugePhase()
        {
            HeroPresenter.AddActionGauge();

            foreach (var enemy in GetAliveEnemyPresenters())
                enemy.AddActionGauge();
        }

        public async UniTask MoveStage(DoorPresenter door)
        {
            door.View.Open();
            await View.MoveStage();
            door.View.Close();
            await UniTask.Delay(1000);
            await GameManager.Instance.LoadStageScene(door.GetStageData());
        }

        public void TargetEnemy(EnemyPresenter ep)
        {
            if (HeroPresenter.GetActionReady())
            {
                _curTarget = ep;
                View.SetTargetIndicator(ep);
            }
            
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
