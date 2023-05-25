using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using Newtonsoft.Json;
using View;

namespace Presenter
{
    public class User : Scene
    {
        public UserModel uModel;
        [JsonIgnore]
        public UserView uView => View as UserView;
        public List<Card> Cards = new();
        public List<Artifact> Artifacts = new();
        public Hero UserHero;

        public User()
        {
        }

        public User(UserModel model, MasterUser mu, MasterTable mt) : base(model)
        {
            uModel = model;
            Init(mu, mt);
        }
        public void Init(MasterUser mu, MasterTable mt)
        {
            uModel.Init(mu, mt);
            UserHero = new Hero(uModel.Hero);

            foreach (var cardModel in uModel.Cards)
            {
                var card = new Card(cardModel);
                Cards.Add(card);
            }

            foreach (var artifactModel in uModel.Artifacts)
            {
                var artifact = new Artifact(artifactModel);
                Artifacts.Add(artifact);
            }
        }

        public override void SetView(SceneView view)
        {
            base.SetView(view);
            
            foreach (var card in Cards)
            {
                card.SetView(uView.CreateDeckCard());
            }

            foreach (var artifact in Artifacts)
            {
                artifact.SetView(uView.CreateArtifactView());
            }
            uView.SetView(this);
        }

        public List<CardModel> GetCards()
        {
            return uModel.Cards;
        }

        public int GetDrawCount()
        {
            return uModel.DrawCardCount;
        }

        public void UseEnergy(int cost)
        {
            uModel.CurEnergy -= cost;
        }

        public void AddEnergy(int value)
        {
            uModel.CurEnergy += value;
        }

        public void AddMaxEnergy(int value)
        {
            uModel.MaxEnergy += value;
        }
        public async UniTask UseCard(Card card, Character target) 
        {
            UseEnergy(card.GetCost());
            await UserHero.PlayAttack();
            await card.CardActivate(UserHero, target);
        }

        public bool CanUseThisCard(Card selectedCard)
        {
            return uModel.CurEnergy >= selectedCard.GetCost();
        }

        public void AddCard(Card card)
        {
            uModel.Cards.Add(card.Model);
            Cards.Add(card);
            card.View = uView.CreateDeckCard();
            card.Init();
        }

        public void SetEnergy()
        {
            uModel.CurEnergy = uModel.MaxEnergy;
        }

        public async UniTask ActivateArtifacts(ArtifactTrigger trigger, object target)
        {
            foreach (var artifact in Artifacts)
            {
                await artifact.Activate(trigger, target);
            }

            foreach (var artifact in Artifacts)
            {
                artifact.Model.IsActive = false;
            }
        }

       

        public void AddGold(int amount)
        {
            var prevGold = uModel.Gold;
            uModel.Gold += amount;
            uView.AddGold(prevGold, amount);
        }

        public void UseGold(int amount)
        {
            var prevGold = uModel.Gold;
            uModel.Gold -= amount;
            uView.AddGold(prevGold, -amount);
        }

        public void AddArtifact(Artifact artifact)
        {
            artifact.SetView(uView.CreateArtifactView());
            artifact.InitFunc(this);
            Artifacts.Add(artifact);
        }
    }
}
