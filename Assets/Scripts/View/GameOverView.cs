using System;
using Manager;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class GameOverView : MonoBehaviour
    {
        public Button Btn_Confirm;

        public void Start()
        {
            Btn_Confirm.onClick.AsObservable().Subscribe((async _ =>
            {
                await GameManager.Instance.ReturnToMain();
            }));
        }
    }
}
