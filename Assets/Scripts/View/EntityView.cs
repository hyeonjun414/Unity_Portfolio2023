using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Model;
using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public interface IEntityView
    {
        void Init(EntityModel entity);
        void UpdateHp(float curHp, float maxHp);

        void UpdateActionGauge(float curActionPoint, float maxActionPoint);
    }
    public class EntityView : MonoBehaviour, IEntityView
    {
        protected const string STR_MOVE = "Move";
        protected const string STR_ATTACK = "Attack";
        protected const string STR_HIT = "Hit";

        public EntityPresenter Presenter;

        public Animator animator;
        
        [Header("EntityUI")] 
        public SpriteRenderer sprite;

        public Canvas uiCanvas;
        public Slider ActionGauge;
        public Slider HpGauge;
        public TextMeshProUGUI HpText;

        public void Init(EntityModel entity)
        {
            Presenter = new EntityPresenter(entity, this);
            InitHp(entity.CurHp, entity.MaxHp);
            InitActionGauge(entity.CurActionGauge, entity.MaxActionGauge);
        }

        private void InitHp(float curHp, float maxHp)
        {
            HpGauge.maxValue = maxHp;
            HpGauge.value = curHp;
            
            HpText.SetText($"{curHp} / {maxHp}");
        }

        public void UpdateHp(float curHp, float maxHp)
        {
            HpGauge.maxValue = maxHp;
            HpGauge.value = curHp;

            HpText.SetText($"{curHp} / {maxHp}");
        }

        private void InitActionGauge(float curActionPoint, float maxActionPoint)
        {
            ActionGauge.maxValue = maxActionPoint;
            ActionGauge.value = curActionPoint;
        }

        public void UpdateActionGauge(float curActionPoint, float maxActionPoint)
        {
            ActionGauge.maxValue = maxActionPoint;
            ActionGauge.value = curActionPoint;
        }

        public virtual async UniTask Attack(EntityView enemyView)
        {
            var moveX = transform.position.x > enemyView.transform.position.x ? -2 : 2;
            transform.DOMoveX(enemyView.transform.position.x - moveX, 0.5f)
                .SetEase(Ease.OutExpo)
                .OnStart(() =>
                {
                    uiCanvas.sortingOrder = sprite.sortingOrder = 5;
                    animator.SetBool(STR_MOVE, true);
                })
                .OnComplete(() => animator.SetBool(STR_MOVE, false));
            await UniTask.Delay(500);
            
            enemyView.Damaged();
            
            transform.DOLocalMove(Vector3.zero, 0.5f)
                .SetEase(Ease.OutExpo)
                .OnStart(() => animator.SetBool(STR_MOVE, true))
                .OnComplete(() =>
                {
                    uiCanvas.sortingOrder = sprite.sortingOrder = 4;
                    animator.SetBool(STR_MOVE, false);
                });
            await UniTask.Delay(500);
        }

        public void Damaged()
        {
            Presenter.Damaged();
        }

        public void PlayDamageEft()
        {
            animator.SetTrigger(STR_HIT);
        }
    }

    
}
