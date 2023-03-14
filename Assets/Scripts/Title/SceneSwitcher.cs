using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Title
{
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private Button button;
        private void Start()
        {
            button.onClick.AsObservable().Subscribe(_ =>
            {
                SceneManager.LoadSceneAsync(sceneName);
            });
        }
    }
}
