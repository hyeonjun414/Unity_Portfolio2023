using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager;
using Model;
using Presenter;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace View.StageView
{
    public class BattleStageView : StageView
    {
        private BattleStage bsPresenter => Presenter as BattleStage;
        
        public TextMeshProUGUI energyText;
        public CharacterView characterView;
        public AllyView allyView;
        public DoorView doorPrefab;
        public CardView cardPrefab;
        public EnemyView enemyPrefab;
        public ChestView chestPrefab;
        public Transform doorPivot, chestPivot;
            
        public GameOverView gameOverView;

        public Button turnEndButton;
        public ActionBar actionBar;

        public CardHolder cardHolder;
        public CharacterHolder characterHolder;
        
        private CharacterView _heroView;

        public override void SetView(Scene scene)
        {
            base.SetView(scene);
            turnEndButton.onClick.AsObservable().Subscribe(async _ => { await bsPresenter.TurnEnd(); });
        }

        public CharacterView CreateHeroView()
        {
            var inst = Instantiate(characterView);
            characterHolder.AddCharacterView(inst);
            _heroView = inst;
            return inst;
        }

        public CharacterView CreateAllyView()
        {
            var inst = Instantiate(allyView);
            characterHolder.AddCharacterView(inst);
            return inst;
        }

        public CharacterView CreateEnemyView()
        {
            var inst = Instantiate(enemyPrefab);
            characterHolder.AddCharacterView(inst);
            return inst;
        }

        public CardView CreateCardView()
        {
            var inst = Instantiate(cardPrefab);
            cardHolder.AddCard(inst);
            return inst;
        }
        
        public void AddApView(Character character)
        {
            actionBar.AddEntity(character);
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
        
        public async UniTask DrawCard(Card card)
        {
            var cardView = card.View;
            if (cardView == null) return;
            
            cardHolder.DrawCard(cardView);
        }

        public async UniTask ReturnToDeck(List<Card> deck)
        {
            cardHolder.Shuffle();
            foreach (var card in deck)
            {
                var cardView = card.View;
                cardHolder.ReturnToDeck(cardView);
                await UniTask.Delay(50);
            }

            
            await UniTask.Delay(250);
        }

        public async UniTask DiscardCard(Card card)
        {
            var cardView = card.View;
            if (cardView == null) return;

            cardHolder.DiscardCard(cardView);
            await UniTask.Delay(50);
        }

        public ChestView GenerateChest()
        {
            var inst = Instantiate(chestPrefab, chestPivot);
            return inst;
        }

        public void SetEnergyText(int userCurEnergy, int userMaxEnergy)
        {
            energyText.SetText($"{userCurEnergy}/{userMaxEnergy}");
        }

        public void TurnEnded()
        {
            turnEndButton.interactable = false;
        }

        public void TurnStarted()
        {
            turnEndButton.interactable = true;
        }

        public void CardHovered(CardView cardView)
        {
            cardHolder.CardHovered(cardView);
        }
        public void CardUnHovered(CardView cardView)
        {
            cardHolder.CardUnHovered(cardView);
        }


        public void CardSelected(CardView cardView)
        {
            cardHolder.CardSelected(cardView);
        }

        public void CardUnSelected(CardView cardView)
        {
            
            cardHolder.CardUnSelected(cardView);
        }

        public async UniTask EntityRemoved(Character character)
        {
            await characterHolder.RemoveCharacterView(character.View);
        }


        public void PositionSwitched(Character character, int targetIndex, int moveIndex)
        {
            characterHolder.PositionSwitched(character, targetIndex, moveIndex);
            
        }

        public void GameOvered()
        {
            gameOverView.gameObject.SetActive(true);
        }
    }
}
