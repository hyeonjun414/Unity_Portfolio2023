using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class ChestView : MonoBehaviour, IPointerClickHandler
    {
        public Animator animator;
        public BoxCollider2D inputChecker;

        [Header("Sound")] 
        public AudioClip openSound;
        public AudioClip closeSound;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (GameManager.Instance.CurStage is BattleStage battleStage)
            {
                var task = battleStage.OpenReward(this);
            }
        }

        public async UniTask Open()
        {
            inputChecker.enabled = false;
            animator.SetTrigger("Open");
            SoundManager.Instance.PlaySfx(openSound);
            await UniTask.Yield();
            await UniTask.Delay((int)(GetCurAnimationDuration() * 1000));
        }

        public async UniTask Close()
        {
            inputChecker.enabled = true;
            animator.SetTrigger("Close");
            SoundManager.Instance.PlaySfx(closeSound);
            await UniTask.Yield();
            await UniTask.Delay((int)(GetCurAnimationDuration() * 1000));
        }

        public async UniTask DestroyView()
        {
            animator.SetTrigger("Close");
            SoundManager.Instance.PlaySfx(closeSound);
            await UniTask.Yield();
            await UniTask.Delay((int)(GetCurAnimationDuration() * 1000));
            gameObject.transform.DOLocalMoveY(-5, 0.5f)
                .SetEase(Ease.InExpo)
                .OnComplete(() => Destroy(gameObject,0.1f))
                .SetRelative();
        }

        public float GetCurAnimationDuration()
        {
            return animator.GetCurrentAnimatorStateInfo(0).length;
        }
    }
}
