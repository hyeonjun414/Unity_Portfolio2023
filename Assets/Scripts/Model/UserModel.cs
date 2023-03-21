using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public class UserModel
    {
        public EntityModel Hero;
        public List<CardModel> Cards = new();

        public int DrawCardCount;

        public void Init(MasterUser mu, MasterTable mt)
        {
            var hero = mt.MasterHeroes.First(target => target.Name == mu.Hero);
            Hero = new EnemyModel(hero);

            DrawCardCount = mu.DrawCardCount;
            
            foreach (var cardId in mu.Cards)
            {
                var mc = mt.MasterCards.First(target => target.Id == cardId);
                Cards.Add(new CardModel(mc));
            }
        }
    }
}
