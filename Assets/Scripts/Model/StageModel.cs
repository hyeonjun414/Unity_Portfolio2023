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
                
                // card
                var cardList = mt.MasterCards.ToList();
                for (var i = 0; i < cardCount; i++)
                {
                    SellCards.Add(new CardModel(cardList.OrderBy(t => Random.value).First()));
                }

                // artifact
                var artifactList = mt.MasterArtifacts.ToList();
                for (var i = 0; i < artifactCount; i++)
                {
                    var newModel = artifactList.OrderBy(t => Random.value).First();
                    SellArtifacts.Add(new ArtifactModel(newModel));
                    Debug.Log(newModel.Name);
                }
                
            }
        }
    }
}
