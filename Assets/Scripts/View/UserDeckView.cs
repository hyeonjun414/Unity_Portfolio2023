using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class UserDeckView : MonoBehaviour
    {
        public Button deckBtn;
        public Transform cardPivot;
        public GameObject deckPanel;
        public TextMeshProUGUI deckCardCount;

        public List<CardView> deckCardViews = new();

        private void Start()
        {
            deckBtn.onClick.AsObservable().Subscribe(_ =>
            {
                deckPanel.SetActive(!deckPanel.activeSelf);
            });
        }

        public void AddCard(CardView inst)
        {
            inst.transform.SetParent(cardPivot);
            deckCardViews.Add(inst);
            deckCardCount.text = deckCardViews.Count.ToString();
        }
    }
}
