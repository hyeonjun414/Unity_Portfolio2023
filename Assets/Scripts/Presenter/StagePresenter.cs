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

        public bool IsAction;


        private GameManager gm;
        public StagePresenter(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            gm = GameManager.Instance;
        }

        public void Init()
        {
            var userHero = GameManager.Instance.user.GetHero();
            View.CreateHeroView(userHero);

            for (var index = 0; index < Model.Enemies.Count; index++)
            {
                View.CreateEnemyView(index, Model.Enemies[index]);
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
            if (gm.user.GetHero().IsActionReady)
            {
                var enemyIndex = Random.Range(0, Model.Enemies.Count);
                Attack(gm.user.GetHero(), Model.Enemies[enemyIndex]);
                IsAction = true;
                await View.HeroView.Attack(View.EnemyViews[enemyIndex]);
                IsAction = false;
            }

            for (var index = 0; index < Model.Enemies.Count; index++)
            {
                var enemy = Model.Enemies[index];
                if (enemy.IsActionReady)
                {
                    Attack(enemy, gm.user.GetHero());
                    IsAction = true;
                    await View.EnemyViews[index].Attack(View.HeroView);
                    IsAction = false;
                }
            }
        }

        private void Attack(EntityModel actor, EntityModel target)
        {
            actor.IsActionReady = false;
            actor.CurActionGauge = 0;
            target.CurHp -= actor.Damage;
        }

        private void UpdateActionGaugePhase()
        {
            var heroModel = gm.user.GetHero();
            heroModel.UpdateActionGauge(Time.deltaTime);
            View.HeroView.UpdateActionGauge(heroModel.CurActionGauge, heroModel.MaxActionGauge);
            
            for (var index = 0; index < Model.Enemies.Count; index++)
            {
                var enemy = Model.Enemies[index];
                enemy.UpdateActionGauge(Time.deltaTime);
                View.EnemyViews[index].UpdateActionGauge(enemy.CurActionGauge, enemy.MaxActionGauge);
            }
        }
        
    }
}
