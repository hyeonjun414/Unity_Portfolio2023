using System.Collections.Generic;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class Reward
    {
        public RewardModel Model;
        public RewardView View;

        public List<Card> Cards;
        
        public Reward(RewardModel model, RewardView view)
        {
            Model = model;
            View = view;
        }

        public void Init(List<Card> cards)
        {
            Cards = cards;
        }
    }

    public class RewardModel
    {
        public List<CardModel> RewardDatas;
    }
}
