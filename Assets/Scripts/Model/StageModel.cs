using System.Collections.Generic;
using System.Linq;
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

        public BattleStageModel(StageInfo stageInfo, MasterTable mt) : base(stageInfo, mt)
        {
            if (stageInfo is BattleStageInfo info)
            {
                foreach (var enemyId in info.Enemies)
                {
                    var masterEnemy = mt.MasterEnemies.First(target => target.Id == enemyId);
                    var enemy = new EnemyModel(masterEnemy);
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
}
