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
        public TextMeshProUGUI turnText, valueText;
        
        public List<Sprite> iconImages;

        public void SetView(StatusEffect statEft)
        {
            Presenter = statEft;
            
            valueText.SetText(Presenter.Model.Value.ToString());
            turnText.SetText(Presenter.Model.Turn.ToString());
            icon.sprite = iconImages.Find(t => t.name == Presenter.Model.Icon);
        }
        
        public async UniTask Activate(int remainTurn)
        {
            turnText.SetText(remainTurn.ToString());
            await UniTask.Delay(200);
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }
    }
}
