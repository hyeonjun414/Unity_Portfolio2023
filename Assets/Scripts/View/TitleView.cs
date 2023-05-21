using System;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TitleView : SceneView
    {
        public Button GameStartBtn;

        public void Start()
        {
            SoundManager.Instance.PlayBgm(true);
            
            GameStartBtn.onClick.AsObservable().Subscribe(async _ =>
            {
                await GameManager.Instance.GameStart();
            });
        }
    }
}
