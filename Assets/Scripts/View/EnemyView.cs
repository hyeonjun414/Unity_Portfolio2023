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
            transform.DOMoveX(moveX, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutExpo)
                .OnStart(() =>
                {
                    sprite.sortingOrder = 5;
                    animator.SetBool("Move", true);
                })
                .OnComplete(() => animator.SetBool("Move", false));
            await UniTask.Delay(500);

            animator.SetTrigger("Attack");
            await UniTask.Yield();
            var curClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            var attackPlayTime = curClip.length;
            var attackPeekTime = curClip.events!.FirstOrDefault(target => target is { functionName: "Attack" })!.time;
            await UniTask.Delay((int)(attackPeekTime * 1000));
            enemyView.Damaged();
            await UniTask.Delay((int)((attackPlayTime - attackPeekTime) * 1000));

            transform.DOMoveX(-moveX, 0.5f)
                .SetRelative()
                .SetEase(Ease.OutExpo)
                .OnStart(() => animator.SetBool("Move", true))
                .OnComplete(() =>
                {
                    sprite.sortingOrder = 4;
                    animator.SetBool("Move", false);
                });

            await UniTask.Delay(500);
        }

        
    }
}
