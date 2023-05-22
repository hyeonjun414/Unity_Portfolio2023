using Manager;
using Model;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class DoorView : MonoBehaviour, IPointerClickHandler
    {
        public Door Presenter;
        public Animator animator;

        [Header("Sound")] 
        public AudioClip openSound;
        public AudioClip closeSound;
        public void Init(Door presenter)
        {
            Presenter = presenter;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            print("Door Click!");
            var task = Presenter.MoveStage();
        }

        public void Open()
        {
            SoundManager.Instance.PlaySfx(openSound);
            animator.SetBool("Open", true);
        }

        public void Close()
        {
            SoundManager.Instance.PlaySfx(closeSound);
            animator.SetBool("Open", false);
        }
    }
}
