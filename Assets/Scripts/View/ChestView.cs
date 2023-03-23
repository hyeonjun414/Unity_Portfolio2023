using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class ChestView : MonoBehaviour, IPointerClickHandler
    {
        public Animator animator;
        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.Instance.CurStage.OpenReward(this);
        }

        public async UniTask Open()
        {
            
        }

        public async UniTask Close()
        {

        }
    }
}
