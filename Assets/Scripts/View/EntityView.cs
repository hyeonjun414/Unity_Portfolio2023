using System;
using System.Linq;
using System.Threading.Tasks;
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

        Vector3 GetPosition();
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

        public void PlayDamageEft()
        {
            animator.SetTrigger(STR_HIT);
        }

        public virtual async UniTask PrepareAttack(Vector3 targetPos)
        {
            var moveX = transform.position.x > targetPos.x ? -2 : 2;
            transform.DOMoveX(targetPos.x - moveX, 0.5f)
                .SetEase(Ease.OutExpo)
                .OnStart(() =>
                {
                    uiCanvas.sortingOrder = sprite.sortingOrder = 5;
                    animator.SetBool(STR_MOVE, true);
                })
                .OnComplete(() => animator.SetBool(STR_MOVE, false));
            await UniTask.Delay(500);
        }

        public virtual async UniTask PlayAttack()
        {
            await UniTask.Yield();
        }

        public virtual async UniTask EndAttack(Vector3 targetPos)
        {
            var moveX = transform.position.x > targetPos.x ? -2 : 2;
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

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void RemoveFromStage()
        {
            //Destroy(gameObject);
            //transform.DOScale(Vector3.one * 0.8f, 0.5f);
            //sprite.DOColor()
            //transform.DOLocalMoveY(-1, 0.5f);
        }

        public async UniTask Dead()
        {
            animator.SetBool("Dead", true);
            animator.SetTrigger(STR_HIT);
            transform.DOScale(Vector3.one * 0.8f, 0.5f);
            sprite.DOColor(Color.gray, 0.5f);
        }
    }

    
}
