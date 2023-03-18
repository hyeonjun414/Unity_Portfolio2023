using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class StageModel
    {
        public List<Enemy> Enemies;

        public StageModel(MasterStage ms, MasterTable mt)
        {
            Enemies = new List<Enemy>();
            
            foreach (var enemyId in ms.StageEnemies)
            {
                var masterEnemy = mt.MasterEnemies.First(target => target.Id == enemyId);
                var enemy = new Enemy();
                enemy.Init(masterEnemy);
                Enemies.Add(enemy);
            }
        }
    }
}
