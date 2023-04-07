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
        public Hero UserHero;

        public User(UserModel model, UserView view, MasterUser mu, MasterTable mt)
        {
            this.Model = model;
            
            Model.Init(mu, mt);
            UserHero = new Hero(Model.Hero, null);
            
            foreach (var cardModel in Model.Cards)
            {
                var card = new BattleCard(cardModel, null);
                Cards.Add(card);
            }
            this.View = view;
            
        }

        public HeroModel GetHeroModel()
        {
            return Model.Hero;
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
        public async UniTask UseCard(BattleCard card, Enemy target) {
            var position = target.View.transform.position;
            //UserHero.UseActionCount(card.GetCost());
            //Model.CurEnergy -= card.GetCost();
            //await UserHero.PrepareAttack(position);
            await UserHero.PlayAttack();
            await card.CardActivate(target);
            //await UserHero.EndAttack();
        }

        public bool CanUseThisCard(BattleCard selectedCard)
        {
            return CurEnergy >= selectedCard.GetCost();
        }

        public void AddCard(Card card)
        {
            Model.Cards.Add(card.Model);
            Cards.Add(card);
        }

        public void SetEnergy()
        {
            Model.CurEnergy = Model.MaxEnergy;
        }

        public int CurEnergy => Model.CurEnergy;
        public int MaxEnergy => Model.MaxEnergy;
    }
}
