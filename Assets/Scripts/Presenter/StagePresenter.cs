using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class StagePresenter
    {
        public StageModel Model;
        public UserModel userModel;
        public StageView View;

        public bool IsAction;

        public StagePresenter(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
            userModel = GameMasterView.Instance.GetUser();
        }

        public void Init()
        {
            View.CreateHeroView(userModel.Hero);

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
            UpdateView();
        }

        private void UpdateView()
        {
            View.UpdateStage();
        }

        private async UniTask AttackPhase()
        {
            if (userModel.Hero.IsActionReady)
            {
                var enemyIndex = Random.Range(0, Model.Enemies.Count);
                Attack(userModel.Hero, Model.Enemies[enemyIndex]);
                IsAction = true;
                await View.HeroAttack(enemyIndex);
                IsAction = false;
            }

            for (var index = 0; index < Model.Enemies.Count; index++)
            {
                var enemy = Model.Enemies[index];
                if (enemy.IsActionReady)
                {
                    Attack(enemy, userModel.Hero);
                    IsAction = true;
                    await View.EnemyAttack(index);
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

        public void UpdateActionGaugePhase()
        {
            userModel.Hero.UpdateActionGauge(Time.deltaTime);
            foreach (var enemy in Model.Enemies)
            {
                enemy.UpdateActionGauge(Time.deltaTime);
            }
        }
        
    }
}
