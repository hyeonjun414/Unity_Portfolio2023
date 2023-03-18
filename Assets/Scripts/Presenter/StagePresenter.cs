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
        }
        
    }
}
