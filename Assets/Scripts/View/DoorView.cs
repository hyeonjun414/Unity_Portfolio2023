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

        public void OnPointerClick(PointerEventData eventData)
        {
            Presenter.OnClick();
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
