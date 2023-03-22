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

        void UpdateActionGauge(float curActionPoint, float maxActionPoint, int actionCount);

        Vector3 GetPosition();
    }
    public class EntityView : MonoBehaviour, IEntityView
    {
        protected const string STR_MOVE = "Move";
        protected const string STR_ATTACK = "Attack";
        protected const string STR_HIT = "Hit";

        public Entity Presenter;

        public Animator animator;
        
        [Header("EntityUI")] 
        public SpriteRenderer sprite;

        public Canvas uiCanvas;
        public Slider ActionGauge;
        public Slider HpGauge;
        public TextMeshProUGUI ActionCountText;
        public TextMeshProUGUI HpText;

        public Image actIcon;
        public TextMeshProUGUI actCost;

        public virtual void Init(EntityModel entity)
        {
            UpdateHp(entity.CurHp, entity.MaxHp);
            UpdateActionGauge(entity.CurActionGauge, entity.MaxActionGauge, entity.ActionCount);
        }

        public void UpdateHp(float curHp, float maxHp)
        {
            HpGauge.maxValue = maxHp;
            HpGauge.value = curHp;

            HpText.SetText($"{curHp} / {maxHp}");
        }
        
        public void UpdateActionGauge(float curActionPoint, float maxActionPoint, int actionCount)
        {
            ActionGauge.maxValue = maxActionPoint;
            ActionGauge.value = curActionPoint;

            var actionCountStr = actionCount.ToString();
            if (ActionCountText.text != actionCountStr)
            {
                ActionCountText.SetText(actionCountStr);
                ActionCountText.transform.DOKill();
                ActionCountText.transform.localScale = Vector3.one;
                ActionCountText.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 1, 0.5f);
            }
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
            animator.SetTrigger("Attack");
            await UniTask.Yield();
            var playTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            await UniTask.Delay((int)(playTime * 1000));
        }

        public virtual async UniTask EndAttack()
        {
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
        
        public async UniTask Dead()
        {
            animator.SetBool("Dead", true);
            animator.SetTrigger(STR_HIT);
            transform.DOScale(Vector3.one * 0.8f, 0.5f);
            sprite.DOColor(Color.gray, 0.5f);
            await UniTask.Yield();
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }

        public void SetActionView(EnemyAction action)
        {
            actIcon.sprite = Resources.Load<Sprite>($"ActionIcon/{action.Icon}");
            actCost.text = action.Cost.ToString();
        }
    }

    
}
