using System;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class SettingView : MonoBehaviour
    {
        [Header("UI")] 
        public GameObject panel;
        public Image sfxIcon, bgmIcon;
        public Slider sfxBar, bgmBar;
        public Button aboutBtn, giveUpBtn, saveQuitBtn, exitBtn, settingBtn;
        
        [Header("Sprite")]
        public Sprite sfxOn;
        public Sprite sfxOf, bgmOn, bgmOff;

        private void SetSfxIcon(float value)
        {
            sfxIcon.sprite = value == 0 ? sfxOf : sfxOn;
        }

        private void SetBgmIcon(float value)
        {
            bgmIcon.sprite = value == 0 ? bgmOff : bgmOn;
        }

        public void SaveVolumeInfo()
        {
            SoundManager.Instance.SaveVolumeData();
        }

        public void Init()
        {
            ObserveReset();
            sfxBar.onValueChanged.AsObservable().Subscribe(_ =>
            {
                SoundManager.Instance.SetSfxVolume(sfxBar.value);
                SetSfxIcon(sfxBar.value);
            });
            bgmBar.onValueChanged.AsObservable().Subscribe(_ =>
            {
                SoundManager.Instance.SetBgmVolume(bgmBar.value);
                SetBgmIcon(bgmBar.value);
            });
            var sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.5f);
            var bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.5f);
            sfxBar.value = sfxVolume;
            bgmBar.value = bgmVolume;

            exitBtn.onClick.AsObservable().Subscribe(_ => 
            {
                SaveVolumeInfo();
                panel.SetActive(false);
            });
            settingBtn.onClick.AsObservable().Subscribe(_ =>
            {
                panel.SetActive(!panel.activeSelf);
            });
            giveUpBtn.onClick.AsObservable().Subscribe(async _ =>
            {
                PlayerPrefs.DeleteKey("SaveData");
                await GameManager.Instance.ReturnToMain();
            });
            saveQuitBtn.onClick.AsObservable().Subscribe(async _ => { await GameManager.Instance.ReturnToMain(); });
        }
        
        private void ObserveReset()
        {
            sfxBar.onValueChanged.RemoveAllListeners();
            bgmBar.onValueChanged.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
            settingBtn.onClick.RemoveAllListeners();
            giveUpBtn.onClick.RemoveAllListeners();
            saveQuitBtn.onClick.RemoveAllListeners();
        }
    }
}
