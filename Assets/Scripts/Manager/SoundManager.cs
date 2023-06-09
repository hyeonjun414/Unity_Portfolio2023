using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Manager
{
    public static class SoundType
    {
        public const string ButtonClick = "ButtonClick";
    }
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        public AudioClip[] bgmClip;
        public float bgmVolume;
        private AudioSource _bgmPlayer;

        private AudioHighPassFilter bgmEffect;

        public AudioClip[] sfxClips;
        public float sfxVolume;
        public int channelNum;
        private List<AudioSource> _sfxPlayers;
        private int _channelIndex;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.5f);
            bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.5f);
            
            bgmClip = Resources.LoadAll<AudioClip>("Sound/Bgm");
            sfxClips = Resources.LoadAll<AudioClip>("Sound/Sfx");
            
            
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            _bgmPlayer = bgmObject.AddComponent<AudioSource>();
            _bgmPlayer.playOnAwake = false;
            _bgmPlayer.bypassListenerEffects = true;
            _bgmPlayer.loop = true;
            _bgmPlayer.volume = bgmVolume;
            _bgmPlayer.clip = bgmClip[0];
            if (Camera.main != null) 
                bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

            GameObject sfxObject = new GameObject("SfxPlayer");
            sfxObject.transform.parent = transform;
            _sfxPlayers = new List<AudioSource>();

            for (int i = 0; i < channelNum; i++)
            {
                var inst = sfxObject.AddComponent<AudioSource>();
                inst.playOnAwake = false;
                inst.volume = sfxVolume;
                inst.bypassListenerEffects = true;
                _sfxPlayers.Add(inst);
            }
        }

        public void PlayBgm(bool isPlay)
        {
            if (isPlay)
                _bgmPlayer.Play();
            else
                _bgmPlayer.Stop();
        }

        public void EffectBgm(bool isPlay)
        {
            bgmEffect.enabled = isPlay;
        }

        public void ChangeBgm(string bgmName)
        {
            _bgmPlayer.clip = sfxClips.First(t => t.name == bgmName);
            _bgmPlayer.Play();
        }

        public void PlaySfx(string sfxName)
        {
            for (int i = 0; i < _sfxPlayers.Count; i++)
            {
                int loopIndex = (i + _channelIndex) % _sfxPlayers.Count;

                if (_sfxPlayers[loopIndex].isPlaying)
                    continue;

                _channelIndex = loopIndex;
                _sfxPlayers[_channelIndex].clip = sfxClips.First(t => t.name == sfxName);
                _sfxPlayers[_channelIndex].Play();
                break;
            } 
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null) return;
            
            for (int i = 0; i < _sfxPlayers.Count; i++)
            {
                int loopIndex = (i + _channelIndex) % _sfxPlayers.Count;

                if (_sfxPlayers[loopIndex].isPlaying)
                    continue;
                
                _channelIndex = loopIndex;
                _sfxPlayers[_channelIndex].clip = clip;
                _sfxPlayers[_channelIndex].Play();
                break;
            } 
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            foreach (var sfxPlayer in _sfxPlayers)
            {
                sfxPlayer.volume = sfxVolume;
            }
        }

        public void SetBgmVolume(float volume)
        {
            bgmVolume = volume;
            _bgmPlayer.volume = bgmVolume;
        }

        public void SaveVolumeData()
        {
            PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
            PlayerPrefs.SetFloat("BgmVolume", bgmVolume);
            PlayerPrefs.Save();
        }
    }
}
