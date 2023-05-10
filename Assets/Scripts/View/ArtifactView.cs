using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class ArtifactView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Artifact Presenter;

        public Image icon;
        public GameObject descPanel;
        public TextMeshProUGUI artifactName, artifactDesc;

        public virtual void SetView(Artifact artifact)
        {
            Presenter = artifact;
            Presenter.View = this;

            var data = artifact.Model;
            artifactName.SetText(data.Name);
            artifactDesc.SetText(data.Desc);
            icon.sprite = Resources.Load<Sprite>($"ArtifactIcon/{data.Icon}");
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            descPanel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            descPanel.SetActive(false);
        }
    }
}
