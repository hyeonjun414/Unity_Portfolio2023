using System.Collections.Generic;
using System.Linq;
using Presenter;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace Model
{
    public enum StageType
    {
        Normal,
        Shop,
        Boss
    }
    
    public class StageModel : SceneModel
    {
        public List<StageModel> NextStages;
        public StageType stageType;
        
        public StageModel(MapNodeModel mapNode, MasterTable mt)
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

        public BattleStageModel(MapNodeModel mapNode, MasterTable mt) : base(
            mapNode, mt)
        {
            if (mapNode.StageData is BattleStageInfo info)
            {
                var enemyCount = Random.Range(info.MinCount, info.MaxCount + 1);
                for (var i = 0; i < enemyCount; i++)
                {
                    var levelValue = Random.Range(mapNode.MinLevelValue, mapNode.MaxLevelValue);
                    var masterEnemy = mt.MasterEnemies.Where(t => t.Selectable).OrderBy(t => Random.value).First();
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

    public class BossStageModel : BattleStageModel
    {
        public BossStageModel(MapNodeModel mapNode, MasterTable mt) : base(mapNode, mt)
        {
            if (mapNode.StageData is BossStageInfo info)
            {
                var bossEnemy = mt.MasterEnemies.Find(t => t.Id == info.BossId);
                var levelValue = Random.Range(mapNode.MinLevelValue, mapNode.MaxLevelValue);
                var enemy = new EnemyModel(bossEnemy, levelValue);
                Enemies.Add(enemy);
            }
        }
    }

    public class ShopStageModel : StageModel
    {
        public List<CardModel> SellCards = new();
        public List<ArtifactModel> SellArtifacts = new();
        
        public ShopStageModel(MapNodeModel mapNode, User user, MasterTable mt) : base(mapNode, mt)
        {
            if (mapNode.StageData is ShopStageInfo info)
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
