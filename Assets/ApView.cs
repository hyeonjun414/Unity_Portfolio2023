using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Presenter;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using View;

public class ApView : MonoBehaviour
{
    private Transform _start, _end;
    private EntityView _connectedView;

    private IDisposable _subcription;

    public void Init(Entity entity, Transform start, Transform end)
    {
        _start = start;
        _end = end;
        _subcription = entity.Model.ApRate.Subscribe(MoveView);
        _connectedView = entity.View;
        _connectedView.gameObject.OnDisableAsObservable().Subscribe(_ =>
        {
            _subcription.Dispose();
            gameObject.SetActive(false);
        });
    }

    public void MoveView(float rate)
    {
        //transform.position = Vector3.Lerp(_start.position, _end.position, rate);
        transform.DOMove(Vector3.Lerp(_start.position, _end.position, rate), 0.2f);
    }
}
