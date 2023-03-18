using System.Collections.Generic;
using Model;
using Presenter;
using UnityEngine;

namespace View
{
    public interface IStageView
    {
        void CreateHeroView(int index, Hero hero);
        void CreateEnemyView(int index, Enemy enemy);
    }
    
    public class StageView : MonoBehaviour, IStageView
    {
        public StagePresenter Presenter;

        public GameObject tempEnemyObj;
        public List<Transform> enemyPosList;
        public List<Transform> heroPosList;
        public void Start()
        {
            if (GameMasterView.Instance == null) 
                return;
            
            var curStage = GameMasterView.Instance.MasterTable.MasterStages[0];
            Presenter = new StagePresenter(new StageModel(curStage, GameMasterView.Instance.MasterTable), this);
            Presenter.Init();
        }

        public void CreateHeroView(int index, Hero hero)
        {
            var inst = Instantiate(tempEnemyObj);
            inst.transform.position = heroPosList[index].position;
            inst.SetActive(true);
        }
        
        public void CreateEnemyView(int index, Enemy enemy)
        {
            var inst = Instantiate(tempEnemyObj);
            inst.transform.position = enemyPosList[index].position;
            inst.SetActive(true);
        }
    }
}
