using System;
using System.Collections;
using System.Collections.Generic;
using Presenter;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using View;

public class ApView : MonoBehaviour
{
    private Transform _start, _end;
    private EntityView _connectedView;

    public void Init(Entity entity, Transform start, Transform end)
    {
        _start = start;
        _end = end;
        entity.Model.ApRate.Subscribe(MoveView);
        _connectedView = entity.View;
        _connectedView.gameObject.OnDestroyAsObservable().Subscribe(_ => Destroy(gameObject));
    }

    public void MoveView(float rate)
    {
        transform.position = Vector3.Lerp(_start.position, _end.position, rate);
    }
}
