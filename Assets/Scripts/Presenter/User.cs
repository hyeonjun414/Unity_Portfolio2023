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
                var card = new Card(cardModel, null);
                Cards.Add(card);
            }
            this.View = view;
            
        }

        public HeroModel GetHeroModel()
        {
            return Model.Hero;
        }

        public List<Card> GetCards()
        {
            return Cards;
        }

        public int GetDrawCount()
        {
            return Model.DrawCardCount;
        }

        public async UniTask UseCard(Card card, Enemy target)
        {
            var position = target.View.transform.position;
            UserHero.UseActionCount(card.GetCost());
            await UserHero.PrepareAttack(position);
            await UserHero.PlayAttack();
            await card.CardActivate(target);
            await UserHero.EndAttack();
        }
    }
}
