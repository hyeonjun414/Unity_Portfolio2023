using System;
using Cysharp.Threading.Tasks;
using Manager;
using Model;
using Newtonsoft.Json;
using UnityEngine;
using View;

namespace Presenter
{
    public enum CardType
    {
        Attack,
        Magic
    }
    public interface IItemState
    {
        void EnterState();
        void OnClick();
        void OnHover();
        void OnUnhover();
        void OnClickDown();
        void OnClickUp();
    }
    
    public class ItemState : IItemState
    {
        public Action OnClickAction;
        public Action OnHoverAction;
        public Action OnUnhoverAction;
        public Action OnClickDownAction;
        public Action OnClickUpAction;

        public void EnterState()
        {
            OnClickAction = null;
            OnHoverAction = null;
            OnUnhoverAction = null;
            OnClickDownAction = null;
            OnClickUpAction = null;
        }

        public void OnClick()
        {
            OnClickAction?.Invoke();
        }

        public void OnHover()
        {
            OnHoverAction?.Invoke();
        }

        public void OnUnhover()
        {
            OnUnhoverAction?.Invoke();
        }

        public void OnClickDown()
        {
            OnClickDownAction?.Invoke();
        }
        public void OnClickUp()
        {
            OnClickUpAction?.Invoke();
        }
    }
    public class Card : Item
    {
        public CardModel Model;
        [JsonIgnore]
        public CardView View;

        public Card(CardModel model)
        {
            Model = model;
            Init();
        }

        public void Init()
        {
            SetState();
        }
        public void SetView(CardView view)
        {
            View = view;
            View.Presenter = this;
            View.SetView(this);
        }

        public CardType GetCardType()
        {
            return Model.CardType;
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
        
        public async UniTask CardActivate(Character hero, Character target)
        {
            await Model.CardActivate(hero, target);
        }
    }

    public class ShopCard : Card
    {
        public ShopCard(CardModel model) : base(model)
        {
        }

        public void Sold()
        {
            if (View is ShopCardView shopCardView)
            {
                shopCardView.Sold();
            }
        }
    }
}
