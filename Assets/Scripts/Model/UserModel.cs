using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public class UserModel
    {
        public HeroModel Hero;
        public List<CardModel> Cards = new();
        public int MaxEnergy;
        public int CurEnergy;

        public int DrawCardCount;

        public void Init(MasterUser mu, MasterTable mt)
        {
            var hero = mt.MasterHeroes.First(target => target.Name == mu.Hero);
            Hero = new HeroModel(hero);

            MaxEnergy = mu.Energy;
            DrawCardCount = mu.DrawCardCount;
            
            foreach (var cardId in mu.Cards)
            {
                Debug.Log(cardId);
                var mc = mt.MasterCards.First(target => target.Id == cardId);
                Cards.Add(new CardModel(mc));
            }
        }
    }
}
