using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class Door
    {
        public DoorModel Model;
        public DoorView View;
        
        public Door(DoorModel model, DoorView view)
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
            var stage = GameManager.Instance.CurStage as BattleStage;
            if (stage != null)
            {
                await stage.MoveStage(this);
            }
        }
    }
}
