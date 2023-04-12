using Cysharp.Threading.Tasks;
using DG.Tweening;
using Presenter;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class BattleCardView : CardView
    {
        public ParticleSystem CardEffect;
        
        public override void SetView(Card card)
        {
            base.SetView(card);
            var data = card.Model;
            CardEffect = Resources.Load<ParticleSystem>($"Particle/{data.Effect}");
        }
    }
}
