using System;
using System.Collections.Generic;
using System.Linq;
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

        public EntityView entityView;
        public DoorView doorPrefab;
        public List<Transform> enemyPosList;
        public Transform heroPosition;
        public Transform doorPosition;

        public EntityView HeroView;
        public List<EnemyView> EnemyViews;
        public List<EnemyView> EnemyPrefabs;

        private bool _isBattleEnd;
        
        public void Start()
        {
            if (GameManager.Instance == null) 
                return;

            EnemyViews = new List<EnemyView>();
            
            var curStage = GameManager.Instance.MasterTable.MasterStages[0];
            Presenter = new StagePresenter(
                new StageModel(curStage, GameManager.Instance.MasterTable),
                this);
            Presenter.Init();
        }

        private async void Update()
        {
            if (_isBattleEnd) return;
            
            await Presenter.Update();
        }

        public void BattleEnd()
        {
            _isBattleEnd = true;
        }

        public void CreateHeroView(EntityModel hero)
        {
            HeroView = Instantiate(entityView, heroPosition);
            HeroView.Init(hero);
            HeroView.gameObject.SetActive(true);
        }
        
        public void CreateEnemyView(int index, EntityModel enemy)
        {
            var enemyView = EnemyPrefabs.First(target => target.name == enemy.Name);
            var inst = Instantiate(enemyView, enemyPosList[index]);
            inst.Init(enemy);
            inst.sprite.flipX = !inst.sprite.flipX;
            inst.gameObject.SetActive(true);
            
            EnemyViews.Add(inst);
        }

        public EntityView GetHeroView()
        {
            return HeroView;
        }

        public List<EnemyView> GetEnemyViews()
        {
            return EnemyViews;
        }

        public DoorView GenerateDoor()
        {
            var doorInst = Instantiate(doorPrefab, doorPosition);

            return doorInst;
        }

        public async UniTask MoveStage()
        {
            HeroView.transform.DOMove(doorPosition.position, 2f)
                .OnStart(() => HeroView.animator.SetBool("Move", true))
                .OnComplete(() => HeroView.animator.SetBool("Move", false));
            await UniTask.Delay(2000);
            HeroView.animator.SetTrigger("DoorIn");
            await UniTask.Yield();
            var clipLength = HeroView.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            HeroView.transform.DOScale(0.8f, clipLength)
                .OnComplete(() => HeroView.gameObject.SetActive(false));
            
            await UniTask.Delay((int)(clipLength * 1000));
            
        }
    }
}
