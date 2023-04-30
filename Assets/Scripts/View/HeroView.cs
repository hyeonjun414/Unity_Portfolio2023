using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class HeroView : EntityView, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerPress != null)
            {
                if (Presenter is Hero hero)
                {
                    hero.Targeted();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerPress != null)
            {
                if (Presenter is Hero hero)
                {
                    hero.UnTargeted();
                }
            }
        }
    }
}
