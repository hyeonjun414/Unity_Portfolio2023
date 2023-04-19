using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Model;
using Presenter;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class EntityView : MonoBehaviour
    {
        protected const string STR_MOVE = "Move";
        protected const string STR_ATTACK = "Attack";
        protected const string STR_HIT = "Hit";

        public Entity Presenter;

        public Animator animator;
        public StatusEffectView statusEffectPrefab;
        public List<StatusEffectView> StatEftList = new();
        public Transform statEftPivot;
        public Transform content;
        [Header("EntityUI")] 
        public SpriteRenderer sprite;

        public Canvas uiCanvas;
        public Slider HpGauge;
        public TextMeshProUGUI HpText;

        public virtual void Init(Entity entity)
        {
            entity.View = this;
            UpdateHp(entity.Model.CurHp, entity.Model.MaxHp);
        }

        public void UpdateHp(float curHp, float maxHp)
        {
            HpGauge.maxValue = maxHp;
            HpGauge.value = curHp;

            HpText.SetText($"{curHp} / {maxHp}");
        }
        
        public void PlayDamageEft()
        {
            animator.SetTrigger(STR_HIT);
        }

        public virtual async UniTask PrepareAttack(Vector3 targetPos)
        {
            var moveX = transform.position.x > targetPos.x ? -2 : 2;
            content.DOMoveX(targetPos.x - moveX, 0.2f)
                .SetEase(Ease.OutExpo)
                .OnStart(() =>
                {
                    uiCanvas.sortingOrder = sprite.sortingOrder = 5;
                    animator.SetBool(STR_MOVE, true);
                })
                .OnComplete(() => animator.SetBool(STR_MOVE, false));
            await UniTask.Delay(200);
        }

        public virtual async UniTask PlayAttack()
        {
            animator.SetTrigger("Attack");
            await UniTask.Yield();
        }

        public virtual async UniTask EndAttack()
        {
            content.DOLocalMove(Vector3.zero, 0.2f)
                .SetEase(Ease.OutExpo)
                .OnStart(() => animator.SetBool(STR_MOVE, true))
                .OnComplete(() =>
                {
                    uiCanvas.sortingOrder = sprite.sortingOrder = 4;
                    animator.SetBool(STR_MOVE, false);
                });
            await UniTask.Delay(200);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        
        public virtual async UniTask Dead()
        {
            animator.SetBool("Dead", true);
            animator.SetTrigger(STR_HIT);
            content.DOScale(Vector3.one * 0.8f, 0.5f);
            sprite.DOColor(Color.gray, 0.5f).OnComplete(()=>gameObject.SetActive(false));
            await UniTask.Yield();
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }


        public async UniTask AddStatusEffect(StatusEffect eft)
        {
            var eftInst = Instantiate(statusEffectPrefab, statEftPivot);
            eftInst.SetView(eft);
            eft.View = eftInst;
            StatEftList.Add(eftInst);
            await UniTask.Yield();
        }
    }

    
}
