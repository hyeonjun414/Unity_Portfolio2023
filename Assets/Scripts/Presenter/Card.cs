using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public class Card
    {
        public CardModel Model;
        public CardView View;

        public Card(CardModel model, CardView view)
        {
            Model = model;
            View = view;
        }
        public void Dispose()
        {
            Model = null;
            View.DestroyView();
        }

        public int GetCost()
        {
            return Model.Cost;
        }
    }

    public class BattleCard : Card
    {
        public BattleCard(CardModel model, CardView view) : base(model, view)
        {
        }

        public void SelectCard()
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.SelectCard(this);
        }

        public void UnSelectCard()
        {
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.UnSelectCard(this);
        }

        public void Selected()
        {
            if (View is BattleCardView bcv)
            {
                bcv.Selected();
            }
        }

        public void UnSelected()
        {
            if (View is BattleCardView bcv)
            {
                bcv.UnSelected();
            }
        }
        
        public async UniTask CardActivate(Enemy enemy)
        {
            if (View is BattleCardView bcv)
            {
                bcv.UnSelected();
                await bcv.PlayCardEft(enemy.View);
                await Model.Function.Activate(enemy);
            }
        }
    }

    public class RewardCard : Card
    {
        public RewardCard(CardModel model, CardView view) : base(model, view)
        {
        }
    }
}
