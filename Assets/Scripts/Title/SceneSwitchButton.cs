using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class SceneSwitchButton : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private Button button;
        private void Start()
        {
            button.onClick.AsObservable().Subscribe( async _ =>
            {
                var sceneSwitcher = GameManager.Instance.sceneSwitcher;
                if (sceneSwitcher != null)
                {
                    await sceneSwitcher.AsyncSceneLoad(sceneName);
                }
            });
        }
    }
}
