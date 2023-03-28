using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class CardView : MonoBehaviour
    {
        public Card Presenter;

        public Image CardImage;
        public GameObject front;
        public GameObject back;
        public TextMeshProUGUI Text_Name;
        public TextMeshProUGUI Text_Desc;
        public TextMeshProUGUI Text_Cost;
        

        private void Update()
        {
            var forward = transform.forward;
            front.SetActive(forward.z > 0);
            back.SetActive(forward.z <= 0);
        }

        public virtual void SetView(Card card)
        {
            Presenter = card;
            
            var data = card.Model;
            Text_Name.SetText(data.Name);
            Text_Desc.SetText(data.Desc);
            Text_Cost.SetText(data.Cost.ToString());
            
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }
        
       
    }
}
