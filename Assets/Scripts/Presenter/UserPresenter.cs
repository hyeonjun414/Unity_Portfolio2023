using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        public EntityPresenter HeroPresenter;

        public UserPresenter(UserModel model, UserView view, MasterUser mu, MasterTable mt)
        {
            this.Model = model;
            
            Model.Init(mu, mt);
            HeroPresenter = new EntityPresenter(Model.Hero, null);
            
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

        public int GetDrawCount()
        {
            return Model.DrawCardCount;
        }

        public async UniTask UseCard(CardPresenter card, EnemyPresenter target)
        {
            var position = target.View.transform.position;
            await HeroPresenter.PrepareAttack(position);
            await HeroPresenter.PlayAttack();
            await card.CardActivate(target);
            await HeroPresenter.EndAttack();
        }
    }
}
