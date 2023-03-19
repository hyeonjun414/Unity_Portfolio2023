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

            for (var index = 0; index < EnemyPresenters.Count; index++)
            {
                var enemy = EnemyPresenters[index];
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
                
            }
            await atker.EndAttack(target.View.GetPosition());
            IsAction = false;
        }

        private void UpdateActionGaugePhase()
        {
            HeroPresenter.AddActionGauge();

            foreach (var enemy in EnemyPresenters)
                enemy.AddActionGauge();
        }
        
    }
}
