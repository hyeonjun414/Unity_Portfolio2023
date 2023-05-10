using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Model;
using Presenter;
using Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class AllyView : CharacterView, IPointerEnterHandler, IPointerExitHandler
    {
        public CharacterActionView actionView;
        
        private float remainAttackAnimTime;
        public override void Init(Character character)
        {
            base.Init(character);
            if (character is Ally ally)
            {
                var enemyData = Resources.Load<CharacterData>($"CharacterData/{ally.Model.Name}");
                if (enemyData != null)
                {
                    animator.runtimeAnimatorController = enemyData.enemyAnim;
                    sprite.flipX = !enemyData.isAlreadyLeft;
                }
            
                gameObject.SetActive(true);
            }
        }

        public override async UniTask Dead()
        {
            await base.Dead();
            actionView.gameObject.SetActive(false);
        }

        public async UniTask SetActionView(CharacterAction action)
        {
            actionView.SetView(action);
            await UniTask.Yield();
        }

        public override async UniTask PlayAttack()
        {
            animator.SetTrigger(STR_ATTACK);
            await UniTask.Yield();
            var curClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            var attackPlayTime = curClip.length;
            var attackPeekTime = curClip.events!.FirstOrDefault(target => target is { functionName: "Attack" })!.time;
            await UniTask.Delay((int)(attackPeekTime * 1000));
            remainAttackAnimTime = (attackPlayTime - attackPeekTime);
        }

        public override async UniTask EndAttack()
        {
            await UniTask.Delay((int)(remainAttackAnimTime * 1000));
            remainAttackAnimTime = 0;
            await base.EndAttack();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (var observer in Observers)
                observer.OnMouseEnterEntity();
            
            if (eventData.pointerPress != null)
            {
                if (Presenter is Enemy ep)
                {
                    ep.Targeted();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var observer in Observers)
                observer.OnMouseExitEntity();
            
            if (eventData.pointerPress != null)
            {
                if (Presenter is Enemy ep)
                {
                    ep.UnTargeted();
                }
            }
        }

        public async UniTask Wait()
        {
            gameObject.transform.DOShakePosition(0.5f, Vector3.one * 0.2f);
            await UniTask.Delay(500);
        }
    }
}