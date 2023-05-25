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

        public SettingView settingView;

        [Header("Sound")] 
        public AudioClip btnClickSound;

        public void Start()
        {
            SceneInit();
        }

        public void SceneInit()
        {
            SoundManager.Instance.PlayBgm(true);

            GameStartBtn.onClick.RemoveAllListeners();
            ContinueBtn.onClick.RemoveAllListeners();
            
            GameStartBtn.onClick.AsObservable().Subscribe(async _ => { await GameManager.Instance.StartGame(); });
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
            else
            {
                ContinueBtn.gameObject.SetActive(false);
            }

            settingView.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();
            }
        }

        private void OnEnable()
        {
            SceneInit();
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
