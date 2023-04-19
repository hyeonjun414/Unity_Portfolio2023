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

        public void Enqueue(UniTask action)
        {
            _queue.Enqueue(action);
        }

        public async UniTask ExecuteAllAction()
        {
            while (_queue.Count > 0)
            {
                if (IsAction)
                {
                    await UniTask.WaitUntil(() => !IsAction);
                }
                var action = _queue.Dequeue();
                IsAction = true;
                await action;
                IsAction = false;
            }
        }

        public virtual async UniTask Update()
        {
            await ExecuteAllAction();
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

        private Enemy _curTarget;
        private Card _selectedCard;
        private Reward _reward;
        public List<Enemy> Enemies = new();
        public List<Card> Hand = new();
        public List<Card> Deck = new();
        public List<Card> Grave = new();
        private bool hasMovedToNextStage;
        private bool rewardGiven;

        private bool _isHeroTurn;
        
        public BattleStage(StageModel model, StageView view) : base(model, view)
        {
        }

        public override void Init()
        {
            base.Init();
            
            bsView.CreateHeroView(gm.User.UserHero);
            bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
            var enemyModels = bsModel.GetEnemies();
            for (var index = 0; index < enemyModels.Count; index++)
            {
                var enemyPresenter = new Enemy(enemyModels[index], null);
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

        public override async UniTask Update()
        {
            await base.Update();
        
            if (_isHeroTurn || IsAction)
            {
                return;
            }

            await AddEntityAp();
            
        }

        private async UniTask AddEntityAp()
        {
            var deltaTime = Time.deltaTime * 5;
            user.UserHero.AddAp(deltaTime);
            if (user.UserHero.Model.IsReady)
            {
                _isHeroTurn = true;
                bsView.isHeroAction = true;
                user.SetEnergy();
                bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
                await DrawCard(user.GetDrawCount());
                return;
            }

            foreach (var enemy in GetAliveEnemies())
            {
                enemy.AddAp(deltaTime);
                if (enemy.Model.IsReady)
                {
                    Enqueue(enemy.ExecuteAction(user.UserHero));
                }
            }
            
        }

        private async UniTask StatusEffectPhase()
        {
            await user.UserHero.StatusEffectActivate();
            foreach (var enemy in GetAliveEnemies())
            {
                await enemy.StatusEffectActivate();
                if (enemy.Model.IsDead)
                    await CheckEnemies();
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


        private async UniTask UseCard(Enemy enemy)
        {
            if (user.CurEnergy >= _selectedCard.GetCost())
            {
                UnTargetEnemy(enemy);
                user.UseEnergy(_selectedCard.GetCost());
                bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
                await user.UseCard(_selectedCard, enemy);
                if (enemy.Model.IsDead)
                {
                    await CheckEnemies();
                }

                await HandToGrave(_selectedCard);
            }
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

        public void TargetEnemy(Enemy ep)
        {
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

        public void SelectCard(Card card)
        {
            Debug.Log("Card click");
            _selectedCard = card;
            bsView.arrow.ActiveArrow(card.View.transform);
        }

        public void UnSelectCard(Card card)
        {
            bsView.arrow.gameObject.SetActive(false);
            if (_selectedCard == card)
            {
                if (_curTarget != null&& user.CanUseThisCard(_selectedCard))
                {
                    var task = UseCard(_curTarget);
                }
                else
                {
                    bsView.UnsetTargetIndicator(); 
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
            bsView.isHeroAction = false;
            user.UserHero.hModel.UseAp();

            // foreach (var enemy in GetAliveEnemies())
            // {
            //     await enemy.StatusEffectActivate();
            //     if (enemy.Model.IsDead)
            //         await CheckEnemies();
            //     else
            //     {
            //         await enemy.ExecuteAction(user.UserHero);
            //     }
            // }

            // user.SetEnergy();
            // bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
            // await DrawCard(user.GetDrawCount());

        }
    }
}
