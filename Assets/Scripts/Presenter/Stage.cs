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
    public enum CharacterType
    {
        Unset,
        Ally,
        Enemy
    }
    public class Stage
    {
        public StageModel Model;
        public StageView View;

        public GameManager gm;
        public User user;
        
        public Stage(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            View.Presenter = this;
            gm = GameManager.Instance;
            user = gm.user;
        }

        public virtual void Init()
        {
        }
        
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
        public List<Character> Allies = new();
        public List<Character> Enemies = new();
        public List<Card> Hand = new();
        public List<Card> Deck = new();
        public List<Card> Grave = new();
        public int ThisTurnUsedCardCount;
        
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
            SetUser();
            SetEnemies();
            StageStart();

            bsView.SetStageView();
        }
        public void SetUser()
        {
            user.UserHero.View = bsView.CreateHeroView();
            user.UserHero.Init();
            user.UserHero.OnDeath += OnDeath;
            bsView.AddApView(user.UserHero);
            Allies.Add(user.UserHero);
            bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
            user.UserHero.UseAp();
            var userCardData = user.GetCards().OrderBy(t => Random.value).ToList();
            foreach (var cardData in userCardData)
            {
                var card = new Card(cardData, bsView.CreateCardView());
                card.Init();
                card.SetState(new CardBattleState());
                Deck.Add(card);
            }
        }
        public void SetEnemies()
        {
            foreach (var enemyModel in bsModel.GetEnemies())
            {
                var enemy = new Enemy(enemyModel, bsView.CreateEnemyView());
                enemy.OnDeath += OnDeath;
                enemy.Init();
                Enemies.Add(enemy);
                bsView.AddApView(enemy);
            }
        }
        public async UniTask SummonAlly(string character, int livingTurn)
        {
            var target = gm.MasterTable.MasterAllies.FirstOrDefault(t => t.Id == character);
            if (target != null)
            {
                var ally = new Ally(new AllyModel(target, livingTurn), bsView.CreateAllyView());
                await user.ActivateArtifacts(ArtifactTrigger.AllySummoned, ally);
                ally.Init();
                ally.OnDeath += OnDeath;
                Allies.Insert(0, ally);
                bsView.AddApView(ally);
            }
        }

        public async void OnDeath(object sender, EventArgs e)
        {
            if (sender is Hero hero)
            {
                Allies.Remove(hero);
                GameOver();
            }
            else if (sender is Ally ally)
            {
                Allies.Remove(ally);
                await RemoveEntityView(ally);
            }
            else if (sender is Enemy enemy)
            {
                UserGetGold(enemy.DropGold);
                Enemies.Remove(enemy);
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
            _isStageClear = true;
            bsView.GameOvered();
        }

        public void StageStart()
        {
            
            var Modeltask = Update();
        }

        public override async UniTask Update()
        {
            await user.ActivateArtifacts(ArtifactTrigger.BattleStarted, this);
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

        public List<Character> GetTarget(Character target, TargetType targetType)
        {
            List<Character> targetList = new();
            switch (target.CharType)
            {
                case CharacterType.Ally:
                    targetList = Allies.ToList();
                    break;
                case CharacterType.Enemy:
                    targetList = Enemies.ToList();
                    break;
            }

            switch (targetType)
            {
                case TargetType.Single:
                    targetList = new List<Character> { target };
                    break;
                case TargetType.All:
                    break;
                case TargetType.Spread:
                    var list = targetList;
                    targetList = targetList.Where(t => Math.Abs(list.IndexOf(target)-list.IndexOf(t)) == 1).ToList();
                    break;
                case TargetType.Random:
                    targetList = new List<Character> { targetList[Random.Range(0, targetList.Count)]};
                    break;
                case TargetType.Front:
                    targetList = new List<Character> { targetList.First() };
                    break;
                case TargetType.Back:
                    targetList = new List<Character> { targetList.Last() };
                    break;
                case TargetType.Hero:
                    targetList = new List<Character> { user.UserHero };
                    break;
            }

            return targetList;
        }
        private async UniTask AddEntityAp()
        {
            var deltaTime = Time.deltaTime;

            foreach (var character in Allies.ToList())
            {
                character.AddAp(deltaTime);
                if (character is Ally ally && ally.Model.IsReady)
                {
                    await ally.PrepareAction();
                    await ally.ExecuteAction(Enemies.First());
                    ally.EndAction();
                }
            }
            
            if (user.UserHero.Model.IsReady)
            {
                await StartUserTurn();
                return;
            }

            foreach (var enemy in GetAliveEnemies())
            {
                enemy.AddAp(deltaTime);
                if (enemy.Model.IsReady)
                {
                    await enemy.PrepareAction();
                    await enemy.ExecuteAction(Allies.First());
                    enemy.EndAction();
                }
            }

            await UniTask.Yield();

        }

        public async UniTask StartUserTurn()
        {
            _isHeroTurn = true;
            user.SetEnergy();
            ThisTurnUsedCardCount = 0;
            bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
            bsView.TurnStarted();
            await user.UserHero.PrepareAction();
            user.UserHero.EndAction();
            await user.ActivateArtifacts(ArtifactTrigger.TurnStarted, this);
            await DrawCard(user.GetDrawCount());
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
            user.UserHero.ResetStat();
            foreach (var ally in Allies.Where(ally => ally is not Hero))
            {
                await RemoveEntityView(ally);
                ally.Dispose();
            }

            while (Hand.Count != 0)
            {
                await HandToGrave(Hand.Last());
            }
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

        private List<Character> GetAliveEnemies()
        {
            return Enemies.Where(enemy => !enemy.Model.IsDead).ToList();
        }

        public async UniTask DrawCard(int drawCount)
        {
            while (drawCount > 0)
            {
                if (Deck.Count > 0)
                {
                    await DeckToHand(Deck.Last());
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
            Hand.Insert(0, card);
            Deck.Remove(card);
            await bsView.DrawCard(card);
        }

        private async UniTask GraveToDeck()
        {
            var shuffledCards = Grave.OrderBy(t => Random.value).ToList();
            Deck.AddRange(shuffledCards);
            Grave.Clear();
            await bsView.ReturnToDeck(Deck);
        }

        private async UniTask HandToGrave(Card card)
        {
            Grave.Add(card);
            Hand.Remove(card);
            await bsView.DiscardCard(card);
        }


        private async UniTask UseCard()
        {
            bsView.CardUnSelected(_selectedCard.View);
            
            switch (_selectedCard.GetCardType())
            {
                case CardType.Attack:
                    if (_curTarget == null) break;
                    ThisTurnUsedCardCount++;
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
            }
        }

        public void UnTargetEntity()
        {
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
                bsView.CardUnSelected(_selectedCard.View);
                _selectedCard = null;
            }
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
            await user.ActivateArtifacts(ArtifactTrigger.TurnEnded, this);
            ThisTurnUsedCardCount = 0;
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


        public async UniTask PositionSwitch(Character character, int moveIndex)
        {
            Character tempTarget;
            int targetIdx = 0;
            int moveIdx = 0;
            var targetList = character is Enemy ? Enemies : Allies;
            targetIdx = targetList.IndexOf(character);
            if (targetIdx == -1) return;
            moveIdx = Math.Clamp(targetIdx + moveIndex, 0, targetList.Count);
            tempTarget = targetList[targetIdx];
            targetList[targetIdx] = targetList[moveIdx];
            targetList[moveIdx] = tempTarget;
            bsView.PositionSwitched(character, targetIdx, moveIdx);
        }

        public async UniTask AddEnergy(int value)
        {
            user.AddEnergy(value);
            bsView.SetEnergyText(user.CurEnergy, user.MaxEnergy);
            await UniTask.Yield();
        }

        public void UserGetGold(int goldAmount)
        {
            user.AddGold(goldAmount);
        }
    }

    public class BossStage : BattleStage
    {
        public BossStage(StageModel model, StageView view) : base(model, view)
        {
        }
    }
    public class ShopStage : Stage
    {
        private ShopStageModel ssModel => Model as ShopStageModel;
        private ShopStageView ssView => View as ShopStageView;

        public List<ShopCard> SellCards = new();
        public List<ShopArtifact> SellArtifacts = new();
        
        public ShopStage(StageModel model, StageView view) : base(model, view)
        {
        }

        public override void Init()
        {
            base.Init();
            ssView.Presenter = this;
            foreach (var cardModel in ssModel.SellCards)
            {
                var card = new ShopCard(cardModel, ssView.CreateCard());
                card.Init();
                card.SetState(new CardShopState());
                card.OnSell += BuyItem;
                SellCards.Add(card);
            }

            foreach (var artifactModel in ssModel.SellArtifacts)
            {
                var artifact = new ShopArtifact(artifactModel, ssView.CreateArtifact());
                artifact.Init(null);
                artifact.OnSell += BuyItem;
                SellArtifacts.Add(artifact);
            }
            ssView.SetStageView();
            
        }

        public void BuyItem(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ShopArtifact artifact:
                    BuyArtifact(artifact);
                    break;
                case ShopCard card:
                    BuyCard(card);
                    break;
            }
        }

        private void BuyArtifact(ShopArtifact artifact)
        {
            if (user.Gold >= artifact.Model.Value)
            {
                artifact.Sold();
                user.UseGold(artifact.Model.Value);
                user.AddArtifact(artifact);
                artifact.OnSell -= BuyItem;
            }
        }

        private void BuyCard(ShopCard card)
        {
            if (user.Gold >= card.Model.Value)
            {
                card.Sold();
                user.UseGold(card.Model.Value);
                user.AddCard(card);
                card.OnSell -= BuyItem;
            }
        }
    }
}
