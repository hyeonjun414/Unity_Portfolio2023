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

        [Header("Sound")] 
        public AudioClip btnClickSound;

        public void Start()
        {
            SoundManager.Instance.PlayBgm(true);
            
            GameStartBtn.onClick.AsObservable().Subscribe(async _ =>
            {
                SoundManager.Instance.PlaySfx(btnClickSound);
                await GameManager.Instance.GameStart();
            });
        }
    }
}
