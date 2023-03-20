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
        private EnemyPresenter _curTarget;
        public StagePresenter(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            gm = GameManager.Instance;
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
            if (HeroPresenter.Model.IsActionReady && _curTarget != null)
            {
                await Attack(HeroPresenter, _curTarget);
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
            {
                if (target == _curTarget)
                {
                    _curTarget = null;
                    View.UnsetTargetIndicator();
                }
                CheckEnemies();
            }
                
            
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

        public void TargetEnemy(EnemyPresenter ep)
        {
            _curTarget = ep;
            View.SetTargetIndicator(ep);
        }
    }
}
