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

        public void Update()
        {
            UpdateActionGaugePhase();
            AttackPhase();
            UpdateView();
        }

        private void UpdateView()
        {
            View.UpdateStage();
        }

        private void AttackPhase()
        {
            if (userModel.Hero.IsActionReady)
            {
                Attack(userModel.Hero, Model.Enemies[Random.Range(0, Model.Enemies.Count)]);
            }

            foreach (var enemy in Model.Enemies)
            {
                if (enemy.IsActionReady)
                {
                    Attack(enemy, userModel.Hero);
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
