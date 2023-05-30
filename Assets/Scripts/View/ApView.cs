using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Presenter;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using View;

public class ApView : MonoBehaviour, ICharacterObserver
{
    private Transform _start, _end;
    private CharacterView _connectedView;

    private IDisposable _subcription;
    public Image icon; 
    public GameObject indicator;

    public void Init(Character character, Transform start, Transform end)
    {
        _start = start;
        _end = end;
        _subcription = character.Model.ApRate.Subscribe(MoveView);
        _connectedView = character.View;
        _connectedView.gameObject.OnDisableAsObservable().Subscribe(_ =>
        {
            _subcription.Dispose();
            gameObject.SetActive(false);
            character.RemoveObserver(this);
        });
        var entityName = character.Model.Name;
        icon.sprite = Resources.Load<Sprite>($"icon/{entityName}");
        character.AddObserver(this);
    }

    public void MoveView(float rate)
    {
        transform.DOMove(Vector3.Lerp(_start.position, _end.position, rate), 0.2f);
    }
    
    public void ActiveIndicator(bool value) => indicator.SetActive(value);
    public void OnMouseEnterEntity()
    {
        transform.SetAsLastSibling();
        ActiveIndicator(true);
    }

    public void OnMouseExitEntity()
    {
        ActiveIndicator(false);
    }
}
