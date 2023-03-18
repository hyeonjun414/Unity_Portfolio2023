using System;
using System.Collections.Generic;
using Model;
using Presenter;
using UnityEngine;

namespace View
{
    public interface IStageView
    {
        void CreateHeroView(EntityModel hero);
        void CreateEnemyView(int index, EntityModel enemy);
    }
    
    public class StageView : MonoBehaviour, IStageView
    {
        public StagePresenter Presenter;

        public GameObject tempEnemyObj;
        public EntityView entityView;
        public List<Transform> enemyPosList;
        public Transform heroPosition;

        public EntityView HeroView;
        public List<EntityView> EnemyViews;
        public void Start()
        {
            if (GameMasterView.Instance == null) 
                return;

            EnemyViews = new List<EntityView>();
            
            var curStage = GameMasterView.Instance.MasterTable.MasterStages[0];
            Presenter = new StagePresenter(
                new StageModel(curStage, GameMasterView.Instance.MasterTable),
                this);
            Presenter.Init();
        }

        private void Update()
        {
            Presenter.Update();
        }

        public void CreateHeroView(EntityModel hero)
        {
            HeroView = Instantiate(entityView);
            HeroView.Init(hero);
            HeroView.transform.position = heroPosition.position;
            HeroView.gameObject.SetActive(true);
        }
        
        public void CreateEnemyView(int index, EntityModel enemy)
        {
            var inst = Instantiate(entityView);
            inst.Init(enemy);
            inst.sprite.flipX = true;
            inst.transform.position = enemyPosList[index].position;
            inst.gameObject.SetActive(true);
            
            EnemyViews.Add(inst);
        }
    }
}
