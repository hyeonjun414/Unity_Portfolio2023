using System.Collections.Generic;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class EnemyActionView : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI valueText;
        public List<Sprite> iconImages;

        private const string IconWait = "Icon_Wait";
        
        public void SetView(EnemyAction ea)
        {
            if (ea.Turn > 0)
            {
                valueText.gameObject.SetActive(false);
                valueText.SetText(ea.GetValue().ToString());
                icon.sprite = iconImages.Find(t => t.name == IconWait);
            }
            else
            {
                valueText.gameObject.SetActive(true);
                valueText.SetText(ea.GetValue().ToString());
                icon.sprite = iconImages.Find(t => t.name == ea.Icon);
            }
        }
    }
}
