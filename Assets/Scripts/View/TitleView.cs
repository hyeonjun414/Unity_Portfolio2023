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
        public Button ContinueBtn;
        public Button QuitBtn;

        [Header("Sound")] 
        public AudioClip btnClickSound;

        public void Start()
        {
            SoundManager.Instance.PlayBgm(true);
            
            GameStartBtn.onClick.AsObservable().Subscribe(async _ =>
            {
                
                await GameManager.Instance.StartGame();
            });
            QuitBtn.onClick.AsObservable().Subscribe(_ =>
            {
                SoundManager.Instance.PlaySfx(btnClickSound);
                Quit();
            });

            if (PlayerPrefs.HasKey("SaveData"))
            {
                ContinueBtn.gameObject.SetActive(true);
                ContinueBtn.onClick.AsObservable().Subscribe(async _ =>
                {
                    SoundManager.Instance.PlaySfx(btnClickSound);
                    await GameManager.Instance.ContinueGame();
                });
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();
            }
        }

        private void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
