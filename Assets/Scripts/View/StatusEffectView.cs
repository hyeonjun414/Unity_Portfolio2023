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
        public ParticleSystem activeEft;
        
        public void SetView()
        {
            activeEft = Resources.Load<ParticleSystem>("Particle/" + Presenter.Model.Particle);
            var statValue = Presenter.Model.Value;
            valueText.SetText(statValue < 1 ? $"{statValue * 100}%" : $"{statValue}");
            turnText.SetText(Presenter.Model.Turn.ToString());
            icon.sprite = iconImages.Find(t => t.name == Presenter.Model.Icon);
        }
        
        public async UniTask Activate(Character character, int remainTurn)
        {
            turnText.SetText(remainTurn.ToString());
            if (activeEft != null)
            {
                await character.PlayEffect(activeEft);
            }
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }
    }
}
