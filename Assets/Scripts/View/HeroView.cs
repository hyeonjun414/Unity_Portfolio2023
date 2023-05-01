using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class HeroView : EntityView, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (var observer in Observers)
                observer.OnMouseEnterEntity();
            
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
            foreach (var observer in Observers)
                observer.OnMouseExitEntity();
            
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
