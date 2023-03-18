using System.Collections.Generic;
using Model;
using Presenter;
using UnityEngine;

namespace View
{
    public interface IStageView
    {
        void CreateHeroView(int index, EntityModel hero);
        void CreateEnemyView(int index, EntityModel enemy);
    }
    
    public class StageView : MonoBehaviour, IStageView
    {
        public StagePresenter Presenter;

        public GameObject tempEnemyObj;
        public EntityView entityView;
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

        public void CreateHeroView(int index, EntityModel hero)
        {
            var inst = Instantiate(entityView);
            inst.Init(hero);
            inst.transform.position = heroPosList[index].position;
            inst.gameObject.SetActive(true);
        }
        
        public void CreateEnemyView(int index, EntityModel enemy)
        {
            var inst = Instantiate(entityView);
            inst.Init(enemy);
            inst.sprite.flipX = true;
            inst.transform.position = enemyPosList[index].position;
            inst.gameObject.SetActive(true);
        }
    }
}
