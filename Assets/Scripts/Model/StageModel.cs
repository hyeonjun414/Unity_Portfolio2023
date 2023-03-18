using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class StageModel
    {
        public List<EntityModel> Enemies;

        public StageModel(MasterStage ms, MasterTable mt)
        {
            Enemies = new List<EntityModel>();
            
            foreach (var enemyId in ms.StageEnemies)
            {
                var masterEnemy = mt.MasterEnemies.First(target => target.Id == enemyId);
                var enemy = new EntityModel(masterEnemy);
                Enemies.Add(enemy);
            }
        }

        public EntityModel GetEntityData(int index)
        {
            return Enemies[index];
        }
    }
}
