using Presenter;
using TMPro;
using UnityEngine;

namespace View
{
    public class ShopCardView : CardView
    {
        public GameObject soldImage;
        public TextMeshProUGUI valueText;

        public override void SetView(Card card)
        {
            base.SetView(card);
            valueText.text = card.Model.Value.ToString();
        }


        public void Sold()
        {
            SetInputChecker(false);
            soldImage.SetActive(true);
        }
    }
}
