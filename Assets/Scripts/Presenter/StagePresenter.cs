using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class StagePresenter
    {
        public StageModel Model;
        public StageView View;

        public StagePresenter(StageModel model, StageView view)
        {
            this.Model = model;
            this.View = view;
        }

        public void Init()
        {
            var user = GameMasterView.Instance.user;
            View.CreateHeroView(user.Hero);

            for (var index = 0; index < Model.Enemies.Count; index++)
            {
                View.CreateEnemyView(index, Model.Enemies[index]);
            }
        }
    }
}
