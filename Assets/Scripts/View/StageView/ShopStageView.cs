using System;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View.StageView
{
    public class ShopStageView : StageView
    {
        public ShopCardView cardPrefab;
        public ShopArtifactView artifactPrefab;

        public Transform cardPivot, artifactPivot;
        public Button quitBtn;
        
        public override void SetView(Scene scene)
        {
            base.SetView(scene);
            quitBtn.onClick.AsObservable().Subscribe(async _ => { await OnQuitBtn(); });
        }

        public async UniTask OnQuitBtn()
        {
            if (Presenter is ShopStage shopPresenter)
            {
                await shopPresenter.StageClear();
            }
            
        }

        public CardView CreateCard()
        {
            var inst = Instantiate(cardPrefab, cardPivot);
            return inst;
        }

        public ArtifactView CreateArtifact()
        {
            var inst = Instantiate(artifactPrefab, artifactPivot);
            return inst;
        }
    }
}
