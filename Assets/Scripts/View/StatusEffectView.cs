using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Model;
using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class StatusEffectView : MonoBehaviour
    {
        public StatusEffect Presenter;

        public Image icon;
        public TextMeshProUGUI turnText;
        
        public List<Sprite> iconImages;

        public void SetView(StatusEffectModel model)
        {
            turnText.SetText(model.GetTurn().ToString());
            icon.sprite = iconImages.Find(t => t.name == model.GetIconName());
        }
        
        public async UniTask Activate(int model)
        {
        }
    }
}
