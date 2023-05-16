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

        private void Start()
        {
            if (GameManager.Instance == null)
                return;


            Presenter = GameManager.Instance.CurStage;
            Presenter.View = this;
            Presenter.Init();

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
