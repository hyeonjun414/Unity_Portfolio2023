using System.Collections.Generic;
using System.Linq;
using View;

namespace Model
{
    public class StageModel
    {
        public List<EnemyModel> Enemies;

        public StageModel(MasterStage ms, MasterTable mt)
        {
            Enemies = new List<EnemyModel>();
            
            foreach (var enemyId in ms.StageEnemies)
            {
                var masterEnemy = mt.MasterEnemies.First(target => target.Id == enemyId);
                var enemy = new EnemyModel(masterEnemy);
                Enemies.Add(enemy);
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
