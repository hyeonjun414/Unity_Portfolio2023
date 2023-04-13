using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
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

        public async UniTask Close()
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            if (curStage != null)
            {
                await curStage.CloseReward(null);
            }
        }
    }

    public class RewardModel
    {
        public List<CardModel> RewardDatas;
    }
}
