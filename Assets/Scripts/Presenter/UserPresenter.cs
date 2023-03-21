using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class UserPresenter
    {
        public UserModel Model;
        public UserView View;

        public List<CardPresenter> Cards = new();

        public UserPresenter(UserModel model, UserView view, MasterUser mu, MasterTable mt)
        {
            this.Model = model;
            var hero = mt.MasterHeroes.First(target => target.Name == mu.Hero);
            List<MasterCard> masterCards = new();
            foreach (var cardId in mu.Cards)
            {
                var mc = mt.MasterCards.First(target => target.Id == cardId);
                masterCards.Add(mc);
            }
            Model.Init(hero, masterCards);

            foreach (var cardModel in Model.Cards)
            {
                var card = new CardPresenter(cardModel, null);
                Cards.Add(card);
            }
            this.View = view;
            
        }

        public EntityModel GetHero()
        {
            return Model.Hero;
        }

        public List<CardPresenter> GetCards()
        {
            return Cards;
        }

        public void SetHero(EntityModel hero)
        {
            throw new System.NotImplementedException();
        }
    }
}
