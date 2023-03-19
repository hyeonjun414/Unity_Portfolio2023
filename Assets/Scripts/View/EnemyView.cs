using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace View
{
    public class EnemyView : EntityView
    {
        public override async UniTask Attack(EntityView enemyView)
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

            animator.SetTrigger(STR_ATTACK);
            await UniTask.Yield();
            var curClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            var attackPlayTime = curClip.length;
            var attackPeekTime = curClip.events!.FirstOrDefault(target => target is { functionName: "Attack" })!.time;
            await UniTask.Delay((int)(attackPeekTime * 1000));
            enemyView.Damaged();
            await UniTask.Delay((int)((attackPlayTime - attackPeekTime) * 1000));

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

        
    }
}
