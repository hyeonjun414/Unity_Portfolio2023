using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class CardPresenter
    {
        public CardModel Model;
        public CardView View;

        public CardPresenter(CardModel model, CardView view)
        {
            Model = model;
            View = view;
        }

        public void SelectCard()
        {
            var curStage = GameManager.Instance.CurStage;
            curStage.SelectCard(this);
        }
        public void UnSelectCard()
        {
            var curStage = GameManager.Instance.CurStage;
            curStage.UnSelectCard(this);
        }
        public void Selected()
        {
            View.Selected();
        }

        public void UnSelected()
        {
            View.UnSelected();
        }


        public async UniTask CardActivate(EntityPresenter entity)
        {
            await View.PlayCardEft(entity.View);
            await Model.Function.Activate(entity);
        }

        public void Dispose()
        {
            Model = null;
            View.DestroyView();
        }
    }
}
