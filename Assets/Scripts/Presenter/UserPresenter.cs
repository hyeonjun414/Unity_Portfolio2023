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
        public List<CardPresenter> Hand = new();
        public List<CardPresenter> Deck = new();
        public List<CardPresenter> Grave = new();

        public UserPresenter(UserModel model, UserView view, MasterUser mu, MasterTable mt)
        {
            this.Model = model;
            
            Model.Init(mu, mt);

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
    }
}
