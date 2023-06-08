using Presenter;
using TMPro;
using UnityEngine;

namespace View
{
    public class ArtifactDescPanel : MonoBehaviour
    {
        public TextMeshProUGUI nameText, descText;
        public Vector3 offset;
        private Artifact _curDescArtifact;
        
        public void SetPanel(Artifact artifact)
        {
            if (_curDescArtifact == artifact && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                _curDescArtifact = null;
                return;
            }

            gameObject.SetActive(true);
            _curDescArtifact = artifact;
            nameText.text = artifact.Model.Name;
            descText.text = artifact.Model.Desc;
            transform.position = artifact.View.WorldPos + offset;
        }
    }
}
