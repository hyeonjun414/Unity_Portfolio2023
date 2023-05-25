using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class Reward : Scene
    {
        public RewardModel Model;
        public RewardView rewardView => View as RewardView;

        public List<Card> Cards;

        public bool sceneClosed;
        public bool rewardSelected;
        public Reward(RewardModel model) : base(model)
        {
            Model = model;
            Cards = new List<Card>();
            Init();
        }

        public void Init()
        {
            var cardTable = gm.MasterTable.MasterCards;
            for (var i = 0; i < 3; i++)
            {
                var cardModel = new CardModel(cardTable[gm.GameCore.Rand.Range(0, cardTable.Count)]);
                var card = new Card(cardModel);
                card.SetState(new CardRewardState());
                Cards.Add(card);
            }

            sceneClosed = false;
            rewardSelected = false;
        }

        public void RewardSelect(Card reward)
        {
            rewardSelected = true;
            gm.user.AddCard(reward);
            Close();
        }

        public override void SetView(SceneView view)
        {
            base.SetView(view);
            rewardView.SetView(this);
        }

        public void Close()
        {
            sceneClosed = true;
            GameManager.Instance.GameCore.CloseCurScene();
        }

        public async UniTask Wait()
        {
            await UniTask.WaitUntil(() => sceneClosed);
        }
    }

    public class RewardModel : SceneModel
    {
    }
}
