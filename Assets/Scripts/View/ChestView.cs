using Cysharp.Threading.Tasks;
using Manager;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class ChestView : MonoBehaviour, IPointerClickHandler
    {
        public Animator animator;
        public void OnPointerClick(PointerEventData eventData)
        {
            var stage = GameManager.Instance.CurStage as BattleStage;
            if (stage != null)
            {
                stage.OpenReward(this);
            }
        }

        public async UniTask Open()
        {
            animator.SetTrigger("Open");
            await UniTask.Yield();
            await UniTask.Delay((int)(GetCurAnimationDuration() * 1000));
        }

        public async UniTask Close()
        {

        }

        public float GetCurAnimationDuration()
        {
            return animator.GetCurrentAnimatorStateInfo(0).length;
        }
    }
}
