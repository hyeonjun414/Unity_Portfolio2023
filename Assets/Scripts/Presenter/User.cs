using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class User
    {
        public UserModel Model;
        public UserView View;

        public List<Card> Cards = new();
        public List<Card> BattleCards = new();
        public List<Artifact> Artifacts = new();
        public Hero UserHero;

        public User(UserModel model, UserView view, MasterUser mu, MasterTable mt)
        {
            this.Model = model;
            this.View = view;
            
            Model.Init(mu, mt);
            UserHero = new Hero(Model.Hero, null);
            
            foreach (var cardModel in Model.Cards)
            {
                var card = new Card(cardModel, null);
                Cards.Add(card);
            }
            foreach (var artifactModel in Model.Artifacts)
            {
                var artifact = new Artifact(artifactModel, null);
                Artifacts.Add(artifact);
            }
        }

        public List<CardModel> GetCards()
        {
            return Model.Cards;
        }

        public int GetDrawCount()
        {
            return Model.DrawCardCount;
        }

        public void UseEnergy(int cost)
        {
            Model.CurEnergy -= cost;
        }

        public void AddEnergy(int value)
        {
            Model.CurEnergy += value;
        }

        public void AddMaxEnergy(int value)
        {
            Model.MaxEnergy += value;
        }
        public async UniTask UseCard(Card card, Character target) 
        {
            UseEnergy(card.GetCost());
            await UserHero.PlayAttack();
            await card.CardActivate(UserHero, target);
        }

        public bool CanUseThisCard(Card selectedCard)
        {
            return CurEnergy >= selectedCard.GetCost();
        }

        public void AddCard(Card card)
        {
            Model.Cards.Add(card.Model);
            Cards.Add(card);
            card.View = View.CreateDeckCard();
            card.Init();
        }

        public void SetEnergy()
        {
            Model.CurEnergy = Model.MaxEnergy;
        }

        public async UniTask ActivateArtifacts(ArtifactTrigger trigger, object target)
        {
            foreach (var artifact in Artifacts)
            {
                await artifact.Activate(trigger, target);
            }
        }

        public int CurEnergy => Model.CurEnergy;
        public int MaxEnergy => Model.MaxEnergy;
        public int Gold => Model.Gold;

        public void AddGold(int amount)
        {
            var prevGold = Model.Gold;
            Model.Gold += amount;
            View.AddGold(prevGold, amount);
        }

        public void UseGold(int amount)
        {
            var prevGold = Model.Gold;
            Model.Gold -= amount;
            View.AddGold(prevGold, -amount);
        }

        public void AddArtifact(Artifact artifact)
        {
            artifact.Init(this);
            Artifacts.Add(artifact);
            View.AddArtifact(artifact);
        }
    }
}
