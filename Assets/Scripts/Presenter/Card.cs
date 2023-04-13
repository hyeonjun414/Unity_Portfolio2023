using Cysharp.Threading.Tasks;
using Manager;
using Model;
using UnityEngine;
using View;

namespace Presenter
{
    public interface ICardState
    {
        void EnterState(Card card);
        void OnClick(Card card);
        void OnHover(Card card);
        void OnUnhover(Card card);
        void OnClickDown(Card card);
        void OnClickUp(Card card);
    }
    public class CardBattleState : ICardState
    {
        public bool IsSelected;
        public void EnterState(Card card)
        {
            IsSelected = false;
        }

        public void OnClick(Card card)
        {
        }

        public void OnHover(Card card)
        {
            if (IsSelected) return;
            card.View.Hovered();
        }

        public void OnUnhover(Card card)
        {
            if (IsSelected) return;
            card.View.Unhovered();
        }

        public void OnClickDown(Card card)
        {
            IsSelected = true;
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.SelectCard(card);
        }

        public void OnClickUp(Card card)
        {
            IsSelected = false;
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.UnSelectCard(card);
        }
    }
    public class Card
    {
        public CardModel Model;
        public CardView View;

        private ICardState _state;


        public Card(CardModel model, CardView view)
        {
            Model = model;
            View = view;
        }

        public void SetState(ICardState newState)
        {
            _state = newState;
            _state.EnterState(this);
        }

        public void OnClick()
        {
            _state.OnClick(this);
        }

        public void OnHover()
        {
            _state.OnHover(this);
        }

        public void OnUnhover()
        {
            _state.OnUnhover(this);
        }

        public void OnClickDown()
        {
            _state.OnClickDown(this);
        }

        public void OnClickUp()
        {
            _state.OnClickUp(this);
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
        
        public async UniTask CardActivate(Enemy enemy)
        {
            View.UnSelected();
            await View.PlayCardEft(enemy.View);
            await Model.CardActivate(enemy);
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

        public async UniTask CardActivate(Enemy enemy)
        {
            if (View is BattleCardView bcv)
            {
                bcv.UnSelected();
                await bcv.PlayCardEft(enemy.View);
                await Model.CardActivate(enemy);
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
