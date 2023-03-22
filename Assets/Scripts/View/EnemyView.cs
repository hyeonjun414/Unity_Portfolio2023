using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Model;
using Presenter;
using Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class EnemyView : EntityView, IPointerEnterHandler, IPointerExitHandler
    {
        private float remainAttackAnimTime;

        public override void Init(EntityModel entity)
        {
            base.Init(entity);
            if (entity is EnemyModel em)
            {
                // Set AnimationController
                var enemyData = Resources.Load<EnemyData>($"EnemyData/{entity.Name}");
                if (enemyData != null)
                {
                    animator.runtimeAnimatorController = enemyData.enemyAnim;
                    sprite.flipX = !enemyData.isAlreadyLeft;
                }

                gameObject.SetActive(true);
            }
        }

        public override async UniTask Dead()
        {
            await base.Dead();
            actIcon.gameObject.SetActive(false);
        }

        public override async UniTask PlayAttack()
        {
            animator.SetTrigger(STR_ATTACK);
            await UniTask.Yield();
            var curClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            var attackPlayTime = curClip.length;
            var attackPeekTime = curClip.events!.FirstOrDefault(target => target is { functionName: "Attack" })!.time;
            await UniTask.Delay((int)(attackPeekTime * 1000));
            remainAttackAnimTime = (attackPlayTime - attackPeekTime);
        }

        public override async UniTask EndAttack()
        {
            await UniTask.Delay((int)(remainAttackAnimTime * 1000));
            remainAttackAnimTime = 0;
            await base.EndAttack();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerPress != null)
            {
                if (Presenter is Enemy ep)
                {
                    ep.Targeted();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerPress != null)
            {
                if (Presenter is Enemy ep)
                {
                    ep.UnTargeted();
                }
            }
        }
    }
}
