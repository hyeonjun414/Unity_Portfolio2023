using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class ArtifactView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Artifact Presenter;

        public Image icon;

        public Image inputChecker;
        public Vector3 WorldPos => transform.position;
        public void SetInputChecker(bool value) => inputChecker.raycastTarget = value;
        
        public virtual void SetView(Artifact artifact)
        {
            Presenter = artifact;
            Presenter.View = this;
            icon.sprite = Resources.Load<Sprite>($"ArtifactIcon/{artifact.Model.Icon}");
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Presenter.OnHover();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Presenter.OnUnhover();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Presenter.OnClick();
        }
    }
}
