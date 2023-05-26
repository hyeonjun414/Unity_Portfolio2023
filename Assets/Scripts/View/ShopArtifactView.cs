using Manager;
using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class ShopArtifactView : ArtifactView
    {
        public GameObject soldImage;
        public TextMeshProUGUI valueText;

        
        public override void SetView(Artifact artifact)
        {
            base.SetView(artifact);
            valueText.text = artifact.Model.Value.ToString();
        }

        public void Sold()
        {
            SetInputChecker(false);
            soldImage.SetActive(true);
        }
    }
}
