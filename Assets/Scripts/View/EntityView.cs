using System;
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
    }
}
