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
            card.View.Hovered(this);
        }

        public void OnUnhover(Card card)
        {
            if (IsSelected) return;
            card.View.Unhovered(this);
        }

        public void OnClickDown(Card card)
        {
            IsSelected = true;
            card.View.Selected(this);
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.SelectCard(card);
        }

        public void OnClickUp(Card card)
        {
            IsSelected = false;
            card.View.UnSelected(this);
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.UnSelectCard(card);
        }
    }

    public class CardRewardState : ICardState
    {
        public void EnterState(Card card)
        {
        }

        public void OnClick(Card card)
        {
            card.View.Selected(this);
            var curStage = GameManager.Instance.CurStage as BattleStage;
            curStage?.CloseReward(card);
        }

        public void OnHover(Card card)
        {
            card.View.Hovered(this);
        }

        public void OnUnhover(Card card)
        {
            card.View.Unhovered(this);
        }

        public void OnClickDown(Card card)
        {
        }

        public void OnClickUp(Card card)
        {
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
            await View.PlayCardEft(enemy.View);
            await Model.CardActivate(enemy);
        }
    }
}
