using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CardView : MonoBehaviour
    {
        public CardPresenter Presenter;

        public Image CardImage;
        public TextMeshProUGUI Text_Name;
        public TextMeshProUGUI Text_Desc;
        public TextMeshProUGUI Text_Damage;

        public void SetView(CardPresenter card)
        {
            Presenter = card;
            var data = card.Model;
            Text_Name.SetText(data.Name);
            Text_Desc.SetText(data.Desc);
        }
    }
}
