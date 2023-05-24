using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace View
{
    public class IntroScene : SceneView, IPointerClickHandler
    {
        public PlayableDirector director;
        public BoxCollider2D inputChecker;
        public SpriteRenderer loadingImage;
        public SignalReceiver signalReceiver;
        public void StartTimeLine()
        {
            director.Play();
        }

        public void StopTimeLine()
        {
            inputChecker.enabled = false;
            signalReceiver.enabled = false;
            GameStart();
            //director.Stop();

        }

        public void GameStart()
        {
            loadingImage.DOColor(Color.black, 0.5f).OnComplete(() =>
            {
                director.time = director.duration;
                gameObject.SetActive(false);
                GameManager.Instance.Init();
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StopTimeLine();
        }
    }
}
