using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class UserModel
    {
        public EntityModel Hero;
        public List<CardModel> Cards = new();

        public void Init(MasterEntity hero, List<MasterCard> masterCards)
        {
            Hero = new EnemyModel(hero);

            foreach (var mc in masterCards)
            {
                Cards.Add(new CardModel(mc));
            }
        }
    }
}
