using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class SceneSwitchButton : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private Button button;
        private void Start()
        {
            button.onClick.AsObservable().Subscribe( async _ =>
            {
                var sceneSwitcher = GameManager.Instance.SceneSwitcher;
                if (sceneSwitcher != null)
                {
                    await sceneSwitcher.AsyncSceneLoad(sceneName);
                }
            });
        }
    }
}
