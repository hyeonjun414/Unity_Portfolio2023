using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class StagePresenter
    {
        public StageModel Model;
        public StageView View;

        public EntityPresenter HeroPresenter;
        public List<EnemyPresenter> EnemyPresenters = new();

        public List<DoorPresenter> Doors = new();
        
        public bool IsAction;


        private GameManager gm;
        public StagePresenter(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            gm = GameManager.Instance;
            gm.CurStage = this;
        }

        public void Init()
        {
            View.CreateHeroView(gm.User.GetHero());
            var heroView = View.GetHeroView();
            HeroPresenter = new EntityPresenter(gm.User.GetHero(), heroView);
            heroView.Presenter = HeroPresenter;

            var enemyModels = Model.GetEnemies();
            for (var index = 0; index < enemyModels.Count; index++)
            {
                View.CreateEnemyView(index, enemyModels[index]);
            }

            var enemyViews = View.GetEnemyViews();
            for (var index = 0; index < enemyViews.Count; index++)
            {
                var enemyPresenter = new EnemyPresenter(enemyModels[index], enemyViews[index]);
                enemyViews[index].Presenter = enemyPresenter;
                EnemyPresenters.Add(enemyPresenter);
            }
        }

        public async UniTask Update()
        {
            if (IsAction) return;
            
            UpdateActionGaugePhase();
            await AttackPhase();
        }

        private void CheckEnemies()
        {
            var allEnemyDead = EnemyPresenters.All(target => target.Model.IsDead);
            if (allEnemyDead)
            {
                foreach (var enemy in EnemyPresenters)
                {
                    enemy.Dispose();
                }

                EnemyPresenters.Clear();
                View.BattleEnd();

                GenerateDoor();
            }
        }

        private void GenerateDoor()
        {
            var masterStage = gm.MasterTable.MasterStages[0];
            var doorModel = new DoorModel(masterStage);
            var doorPresenter = new DoorPresenter(doorModel, View.GenerateDoor());
            
            Doors.Add(doorPresenter);
        }

        private async UniTask AttackPhase()
        {
            if (HeroPresenter.Model.IsActionReady)
            {
                var enemy = EnemyPresenters.Where(target => !target.Model.IsDead).ToList();
                if (enemy.Count != 0)
                {
                    var targetEnemyIndex = Random.Range(0, enemy.Count);
                    await Attack(HeroPresenter, enemy[targetEnemyIndex]);
                }
            }

            foreach (var enemy in EnemyPresenters)
            {
                if (enemy.Model.IsActionReady && !enemy.Model.IsDead)
                    await Attack(enemy, HeroPresenter);
            }
        }

        private async UniTask Attack(EntityPresenter atker, EntityPresenter target)
        {
            IsAction = true;
            await atker.PrepareAttack(target.View.GetPosition());
            await atker.PlayAttack();
            await target.TakeDamage(atker.Model.Damage);
            if (target.Model.IsDead)
                CheckEnemies();
            
            await atker.EndAttack(target.View.GetPosition());
            IsAction = false;
        }

        private void UpdateActionGaugePhase()
        {
            HeroPresenter.AddActionGauge();

            foreach (var enemy in EnemyPresenters.Where(target => !target.Model.IsDead))
                enemy.AddActionGauge();
        }

        public async UniTask MoveStage()
        {
            await View.MoveStage();
        }
    }
}
