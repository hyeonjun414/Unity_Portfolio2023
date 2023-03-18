using System;
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
        void InitHp(float curHp, float maxHp);
        void UpdateHp(float curHp, float maxHp);

        void InitActionGauge(float curActionPoint, float maxActionPoint);
        void UpdateActionGauge(float curActionPoint, float maxActionPoint);
    }
    public class EntityView : MonoBehaviour, IEntityView
    {
        public EntityPresenter Presenter;

        public Animator animator;

        [Header("EntityUI")] 
        public SpriteRenderer sprite;
        public Slider ActionGauge;
        public Slider HpGauge;
        public TextMeshProUGUI HpText;
        
        public void Init(EntityModel entity)
        {
            Presenter = new EntityPresenter(entity, this);
            InitHp(entity.CurHp, entity.MaxHp);
            InitActionGauge(entity.CurActionGauge, entity.MaxActionGauge);
        }

        public void UpdateEntityInfo()
        {
            Presenter.UpdateEntityInfo();
        }

        public void InitHp(float curHp, float maxHp)
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

        public void InitActionGauge(float curActionPoint, float maxActionPoint)
        {
            ActionGauge.maxValue = maxActionPoint;
            ActionGauge.value = curActionPoint;
        }

        public void UpdateActionGauge(float curActionPoint, float maxActionPoint)
        {
            ActionGauge.maxValue = maxActionPoint;
            ActionGauge.value = curActionPoint;
        }

        public async UniTask Attack(EntityView enemyView)
        {
            animator.SetBool("Attack", true);
            var moveX = transform.position.x > enemyView.transform.position.x ? -2 : 2;
            transform.DOMoveX(moveX, 0.1f)
                .SetRelative()
                .SetEase(Ease.OutExpo)
                .SetLoops(2, LoopType.Yoyo)
                .OnStart(() => sprite.sortingOrder = 5)
                .OnComplete(() => sprite.sortingOrder = 4);
            await UniTask.Delay(100);
            enemyView.UpdateEntityInfo();
            await UniTask.Delay(100);
            animator.SetBool("Attack", false);
        }
    }
}
