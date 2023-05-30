using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Manager;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class MapNodeView : MonoBehaviour
    {
        public MapNode Presenter;

        public Image icon;
        public Button button;
        public List<Sprite> iconSprites;

        public AudioClip nodeClickSound;
        public void SetView(MapNode mapNode)
        {
            var iconImage = iconSprites.FirstOrDefault(t => t.name == mapNode.GetIconName());
            icon.sprite = iconImage;
            icon.color = Color.black;
            button.interactable = false;
            button.onClick.AsObservable().Subscribe(async _ =>
            {
                SoundManager.Instance.PlaySfx(nodeClickSound);
                await Presenter.SelectMapNode();
            });
        }

        public void ActiveNode(bool isActive)
        {
            if (isActive)
            {
                icon.DOColor(Color.white, 0.5f)
                    .OnComplete(() => button.interactable = true);
            }
            else
            {
                icon.DOColor(Color.black, 0.5f)
                    .OnStart(() => button.interactable = false);
            }
            
        }

        public void ClearMapNode()
        {
            icon.DOColor(Color.gray, 0.5f)
                .OnStart(() => button.interactable = false);
        }
    }
}
