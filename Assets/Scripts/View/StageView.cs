using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        private async void Update()
        {
            await Presenter.Update();
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

        public void UpdateEntityInfo(EntityView ev)
        {
            ev.UpdateEntityInfo();
        }

        public void UpdateStage()
        {
            UpdateEntityInfo(HeroView);

            foreach (var ev in EnemyViews)
            {
                UpdateEntityInfo(ev);
            }
        }

        public async UniTask HeroAttack(int enemyIdx)
        {
            HeroView.animator.SetBool("Attack", true);
            HeroView.transform.DOMoveX(2, 0.1f)
                .SetRelative()
                .SetEase(Ease.OutExpo)
                .SetLoops(2, LoopType.Yoyo);
            await UniTask.Delay(100);
            UpdateEntityInfo(EnemyViews[enemyIdx]);
            await UniTask.Delay(100);
            HeroView.animator.SetBool("Attack", false);
        }

        public async UniTask EnemyAttack(int index)
        {
            EnemyViews[index].animator.SetBool("Attack", true);
            EnemyViews[index].transform.DOMoveX(-2, 0.1f)
                .SetRelative()
                .SetEase(Ease.OutExpo)
                .SetLoops(2, LoopType.Yoyo);
            await UniTask.Delay(100);
            UpdateEntityInfo(HeroView);
            await UniTask.Delay(100);
            EnemyViews[index].animator.SetBool("Attack", false);
        }
    }
}
