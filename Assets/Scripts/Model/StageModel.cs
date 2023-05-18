using System.Collections.Generic;
using System.Linq;
using Presenter;
using UnityEngine;
using View;

namespace Model
{
    public class StageModel
    {
        public List<StageModel> NextStages;
        
        public StageModel(StageInfo stageInfo, MasterTable mt)
        {
            NextStages = new List<StageModel>();
        }

        public void AddNextStage(StageModel stage)
        {
            NextStages.Add(stage);
        }
    }

    public class BattleStageModel : StageModel
    {
        public List<EnemyModel> Enemies = new();

        public BattleStageModel(StageInfo stageInfo, float minLevelValue, float maxLevelValue, MasterTable mt) : base(stageInfo, mt)
        {
            if (stageInfo is BattleStageInfo info)
            {
                var enemyCount = Random.Range(info.MinCount, info.MaxCount + 1);
                for (var i = 0; i < enemyCount; i++)
                {
                    var levelValue = Random.Range(minLevelValue, maxLevelValue);
                    var masterEnemy = mt.MasterEnemies.OrderBy(t => Random.value).First();
                    var enemy = new EnemyModel(masterEnemy, levelValue);
                    Enemies.Add(enemy);
                }
            }
        }

        public List<EnemyModel> GetEnemies()
        {
            return Enemies;
        }

        public bool AreAllEnemiesDead()
        {
            return Enemies.All(target => target.IsDead);
        }
    }

    public class ShopStageModel : StageModel
    {
        public List<CardModel> SellCards = new();
        public List<ArtifactModel> SellArtifacts = new();
        
        public ShopStageModel(StageInfo stageInfo, User user, MasterTable mt) : base(stageInfo, mt)
        {
            if (stageInfo is ShopStageInfo info)
            {
                var artifactCount = info.ArtifactCount;
                var cardCount = info.CardCount;
                float randomValue;
                // card
                var cardList = mt.MasterCards.ToList();
                for (var i = 0; i < cardCount; i++)
                {
                    var masterCard = cardList.OrderBy(t => Random.value).First();
                    if (masterCard != null)
                    {
                        randomValue = Random.Range(0.8f, 1.2f);
                        var cardModel = new CardModel(masterCard);
                        cardModel.Value = (int)(cardModel.Value * randomValue);
                        SellCards.Add(cardModel);
                        cardList.Remove(masterCard); 
                    }
                }

                // artifact
                var artifactList = mt.MasterArtifacts.ToList();
                foreach (var artifact in user.Artifacts)
                {
                    artifactList.RemoveAll(t => t.Id == artifact.Id);
                }
                for (var i = 0; i < artifactCount; i++)
                {
                    randomValue = Random.Range(0.8f, 1.2f);
                    var newModel = artifactList.OrderBy(t => Random.value).FirstOrDefault();
                    if (newModel != null)
                    {
                        var artifactModel = new ArtifactModel(newModel);
                        artifactModel.Value = (int)(artifactModel.Value * randomValue);
                        SellArtifacts.Add(artifactModel);
                        artifactList.Remove(newModel);
                        Debug.Log(newModel.Name);
                    }
                }
                
            }
        }
    }
}
