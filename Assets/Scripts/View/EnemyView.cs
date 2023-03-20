using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class EnemyView : EntityView, IPointerClickHandler
    {
        private float remainAttackAnimTime;
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

        public override async UniTask EndAttack(Vector3 targetPos)
        {
            await UniTask.Delay((int)(remainAttackAnimTime * 1000));
            remainAttackAnimTime = 0;
            await base.EndAttack(targetPos);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(Presenter is EnemyPresenter ep)
            {
                ep.Targeted();
            }
        }
    }
}
