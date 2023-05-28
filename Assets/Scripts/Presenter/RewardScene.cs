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
        public RewardSceneView RewardSceneView => View as RewardSceneView;

        public List<Item> Rewards;

        public bool SceneClosed;
        public bool RewardSelected;
        public RewardScene(RewardModel model) : base(model)
        {
            Model = model;
            Rewards = new List<Item>();
            Init();
        }

        public void Init()
        {
            SceneClosed = false;
            RewardSelected = false;
        }

        public void SetReward(List<Item> itemList)
        {
            Rewards = itemList;
            foreach (var item in itemList)
            {
                item.State.OnClickAction += () => RewardSelect(item);
            }
        }

        public void RewardSelect(Item reward)
        {
            RewardSelected = true;
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
            SceneClosed = true;
            GameManager.Instance.GameCore.CloseCurScene();
        }

        public async UniTask Wait()
        {
            await UniTask.WaitUntil(() => SceneClosed);
        }
    }

    public class RewardModel : SceneModel
    {
    }
}
