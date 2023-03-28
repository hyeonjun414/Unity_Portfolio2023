using Cysharp.Threading.Tasks;
using DG.Tweening;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class BattleCardView : CardView, IPointerUpHandler, IPointerDownHandler
    {
        public void Selected()
        {
            transform.DOScale(1.2f, 0.1f);
            transform.DOMoveY(50, 0.1f).SetRelative();
        }

        public void UnSelected()
        {
            transform.DOScale(1f, 0.1f);
            transform.DOMoveY(-50, 0.1f).SetRelative();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Presenter is BattleCard bc)
            {
                bc.SelectCard();
            }
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Presenter is BattleCard bc)
            {
                bc.UnSelectCard();
            }
        }

        public async UniTask PlayCardEft(EntityView ev)
        {
            var eft = Instantiate(CardEffect, ev.transform);
            eft.transform.position += Vector3.up;
            Destroy(eft.gameObject, eft.main.duration);
            await UniTask.Yield();
        }
    }
}
