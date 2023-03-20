using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class DoorPresenter
    {
        public DoorModel Model;
        public DoorView View;
        
        public DoorPresenter(DoorModel model, DoorView view)
        {
            Model = model;
            View = view;
            View.Init(this);
        }

        public MasterStage GetStageData()
        {
            return Model.stageData;
        }

        public async UniTaskVoid MoveStage()
        {
            View.Open();
            await GameManager.Instance.CurStage.MoveStage();
            View.Close();
            await UniTask.Delay(1000);
            await GameManager.Instance.LoadStageScene(Model.stageData);
        }
    }
}
