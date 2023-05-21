using System;
using Manager;
using Model;
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
        
        public override void SetStageView()
        {
            base.SetStageView();
            quitBtn.onClick.AsObservable().Subscribe(async _ => { await Presenter.StageClear(); });
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
