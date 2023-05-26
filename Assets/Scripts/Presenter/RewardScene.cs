using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class RewardScene : Scene
    {
        public RewardModel Model;
        public RewardSceneView RewardSceneView => View as RewardSceneView;

        public List<Item> Rewards;

        public bool sceneClosed;
        public bool rewardSelected;
        public RewardScene(RewardModel model) : base(model)
        {
            Model = model;
            Rewards = new List<Item>();
            Init();
        }

        public void Init()
        {
            // var cardTable = gm.MasterTable.MasterCards;
            // for (var i = 0; i < 3; i++)
            // {
            //     var cardModel = new CardModel(cardTable[gm.GameCore.Rand.Range(0, cardTable.Count)]);
            //     var card = new Card(cardModel);
            //     card.State.OnClickAction += () => RewardSelect(card);
            //     Rewards.Add(card);
            // }

            var artifactTable = gm.MasterTable.MasterArtifacts;
            for (var i = 0; i < 3; i++)
            {
                var cardModel = new ArtifactModel(artifactTable[gm.GameCore.Rand.Range(0, artifactTable.Count)]);
                var card = new Artifact(cardModel);
                card.State.OnClickAction += () => RewardSelect(card);
                Rewards.Add(card);
            }

            sceneClosed = false;
            rewardSelected = false;
        }

        public void RewardSelect(Item reward)
        {
            rewardSelected = true;
            Rewards.Remove(reward);
            switch (reward)
            {
                case Card card:
                    gm.user.AddCard(card);
                    card.SetState();
                    break;
                case Artifact artifact:
                    gm.user.AddArtifact(artifact);
                    artifact.SetState();
                    break;
            }
            Close();
        }

        public override void SetView(SceneView view)
        {
            base.SetView(view);
            RewardSceneView.SetView(this);
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
