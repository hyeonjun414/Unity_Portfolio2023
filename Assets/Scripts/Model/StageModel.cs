using System.Collections.Generic;
using System.Linq;
using Manager;
using Presenter;
namespace Model
{
    public enum StageType
    {
        Normal,
        Shop,
        Boss,
        Chest
    }
    
    public class StageModel : SceneModel
    {
        public List<StageModel> NextStages;
        public StageType stageType;
        
        public StageModel()
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
        public List<CardModel> Rewards = new();

        public BattleStageModel()
        {
        }

        public BattleStageModel(MapNodeModel mapNode, MasterTable mt)
        {
            if (mapNode.StageData is BattleStageInfo info)
            {
                var enemyCount = GameManager.Instance.Rand.Range(info.EnemyMinCount, info.EnemyMaxCount + 1);
                var enemyList = mt.MasterEnemies.Where(t => t.Selectable).ToList();
                for (var i = 0; i < enemyCount; i++)
                {
                    var levelValue = GameManager.Instance.Rand.Range(mapNode.MinLevelValue, mapNode.MaxLevelValue);
                    var masterEnemy = enemyList.OrderBy(t => GameManager.Instance.Rand.Value).First();
                    var enemy = new EnemyModel(masterEnemy, levelValue);
                    Enemies.Add(enemy);
                }
                
                var cardList = mt.MasterCards.ToList();
                for (var i = 0; i < info.RewardCardCount; i++)
                {
                    var masterCard = cardList.OrderBy(t => GameManager.Instance.Rand.Value).First();
                    var card = new CardModel(masterCard);
                    cardList.Remove(masterCard);
                    Rewards.Add(card);
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
        public BossStageModel()
        {
            
        }
        public BossStageModel(MapNodeModel mapNode, MasterTable mt) : base(mapNode, mt)
        {
            if (mapNode.StageData is BossStageInfo info)
            {
                var bossEnemy = mt.MasterEnemies.Find(t => t.Id == info.BossId);
                var levelValue = GameManager.Instance.Rand.Range(mapNode.MinLevelValue, mapNode.MaxLevelValue);
                var enemy = new EnemyModel(bossEnemy, levelValue);
                Enemies.Add(enemy);
            }
        }
    }

    public class ShopStageModel : StageModel
    {
        public List<CardModel> SellCards = new();
        public List<ArtifactModel> SellArtifacts = new();

        public ShopStageModel()
        {
        }
        
        public ShopStageModel(MapNodeModel mapNode, User user, MasterTable mt)
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
                    var masterCard = cardList.OrderBy(t => GameManager.Instance.Rand.Value).First();
                    if (masterCard != null)
                    {
                        randomValue = GameManager.Instance.Rand.Range(0.8f, 1.2f);
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
                    randomValue = GameManager.Instance.Rand.Range(0.8f, 1.2f);
                    var newModel = artifactList.OrderBy(t => GameManager.Instance.Rand.Value).FirstOrDefault();
                    if (newModel != null)
                    {
                        var artifactModel = new ArtifactModel(newModel);
                        artifactModel.Value = (int)(artifactModel.Value * randomValue);
                        SellArtifacts.Add(artifactModel);
                        artifactList.Remove(newModel);
                    }
                }
                
            }
        }
    }

    public class ChestStageModel : StageModel
    {
        public List<ArtifactModel> RewardArtifact = new();

        public ChestStageModel()
        {
        }
        
        public ChestStageModel(MapNodeModel mapNode, User user, MasterTable mt)
        {
            if (mapNode.StageData is ChestStageInfo info)
            {
                var artifactCount = info.ArtifactCount;
                
                var artifactList = mt.MasterArtifacts.ToList();
                foreach (var artifact in user.Artifacts)
                {
                    artifactList.RemoveAll(t => t.Id == artifact.Id);
                }
                for (var i = 0; i < artifactCount; i++)
                {
                    var newModel = artifactList.OrderBy(t => GameManager.Instance.Rand.Value).FirstOrDefault();
                    if (newModel == null) continue;
                    
                    var artifactModel = new ArtifactModel(newModel);
                    RewardArtifact.Add(artifactModel);
                    artifactList.Remove(newModel);
                }
                
            }
        }
    }
}
