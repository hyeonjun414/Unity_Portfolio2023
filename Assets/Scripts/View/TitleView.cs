using System;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TitleView : MonoBehaviour
    {
        public Button GameStartBtn;

        public void Start()
        {
            GameStartBtn.onClick.AsObservable().Subscribe(async _ =>
            {
                await GameManager.Instance.GameStart();
            });
        }
    }
}
