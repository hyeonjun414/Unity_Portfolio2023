using Cysharp.Threading.Tasks;
using DG.Tweening;
using Presenter;
using UnityEngine;

namespace View.StageView
{
    public class ChestStageView : StageView
    {
        public HeroView heroPrefab;
        public ChestView chestPrefab;
        public DoorView doorPrefab;
        public Transform chestPivot, doorPivot;
        public CharacterHolder characterHolder;

        private HeroView _heroView;
        
        public override void SetStageView()
        {
            base.SetStageView();
            
        }

        public CharacterView CreateHeroView()
        {
            var inst = Instantiate(heroPrefab);
            characterHolder.AddCharacterView(inst);
            _heroView = inst;
            return inst;
        }

        public DoorView GenerateDoor()
        {
            var doorInst = Instantiate(doorPrefab, doorPivot);
            doorInst.transform.localPosition = Vector3.down * 5;
            doorInst.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutExpo);
            return doorInst;
        }

        public async UniTask MoveStage()
        {
            _heroView.content.transform.DOMove(doorPivot.position, 1f)
                .OnStart(() => _heroView.animator.SetBool("Move", true))
                .OnComplete(() => _heroView.animator.SetBool("Move", false));
            await UniTask.Delay(1000);
            _heroView.animator.SetTrigger("DoorIn");
            await UniTask.Yield();
            var clipLength = _heroView.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / _heroView.animator.speed;
            _heroView.content.transform.DOScale(0.8f, clipLength)
                .OnComplete(() => _heroView.gameObject.SetActive(false));

            await UniTask.Delay((int)(clipLength * 1000));
        }

        public ChestView GenerateChest()
        {
            var inst = Instantiate(chestPrefab, chestPivot);
            return inst;
        }

    }
}
