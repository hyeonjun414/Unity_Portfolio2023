using Model;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class DoorView : MonoBehaviour, IPointerClickHandler
    {
        public DoorPresenter Presenter;
        public Animator animator;

        public void Init(DoorPresenter presenter)
        {
            Presenter = presenter;
            var stageData = Presenter.GetStageData();
            
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            print("Door Click!");
            Presenter.MoveStage();
        }

        public void Open()
        {
            animator.SetBool("Open", true);
        }

        public void Close()
        {
            animator.SetBool("Open", false);
        }
    }
}
