using System.Collections.Generic;
using Presenter;
using UnityEngine;

namespace View
{
    public class StatusEffectView : MonoBehaviour
    {
        public StatusEffect Presenter;

        public SpriteRenderer spriteRenderer;
        
        public List<Sprite> iconImages;
    }
}
