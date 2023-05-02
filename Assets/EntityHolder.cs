using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Presenter;
using UnityEngine;
using View;

public class EntityHolder : MonoBehaviour
{
    public List<EntityView> heroViews;
    public List<EntityView> enemyViews;

    public Transform heroPivot;
    public Transform enemyPivot;
    
    public float areaDistance;
    public float entityDistance;

    public float lerpTime, yOffset;
    private Vector3 targetPos, targetScl;
    private Quaternion targetRot;
    
    public void AddEntityView(EntityView ev)
    {
        var trans = ev.transform;
        switch (ev)
        {
            case HeroView:
                trans.SetParent(heroPivot);
                trans.localPosition += new Vector3(-20, yOffset, 0);
                heroViews.Add(ev);
                break;
            case EnemyView:
                trans.SetParent(enemyPivot);
                trans.localPosition += new Vector3(20, yOffset, 0);
                enemyViews.Add(ev);
                break;
        }
    }

    public async UniTask RemoveEntityView(EntityView ev)
    {
        switch (ev)
        {
            case HeroView:
                await ev.Dead();
                heroViews.Remove(ev);
                break;
            case EnemyView:
                await ev.Dead();
                enemyViews.Remove(ev);
                break;
        }
    }

    private void LateUpdate()
    {
        SetEntityPosition();
    }

    private void SetEntityPosition()
    {
        var lerpAmount = lerpTime * Time.deltaTime;
        for (var i = 0; i < heroViews.Count; i++)
        {
            float offsetX = -areaDistance / 2f - (i - (heroViews.Count - 1) / 2f) * entityDistance;
            targetPos = new Vector3(offsetX, yOffset, 0);
            targetRot = Quaternion.identity;
            targetScl = Vector3.one; 
            var hvTrans = heroViews[i].transform;
            hvTrans.localPosition = Vector3.Lerp(hvTrans.localPosition, targetPos, lerpAmount);
            hvTrans.localRotation = targetRot;
            hvTrans.localScale = targetScl;
        }

        for (var i = 0; i < enemyViews.Count; i++)
        {
            float offsetX = areaDistance / 2f - (i - (enemyViews.Count - 1) / 2f) * entityDistance;
            targetPos = new Vector3(offsetX, yOffset, 0);
            targetRot = Quaternion.identity;
            targetScl = Vector3.one;
            var evTrans = enemyViews[i].transform;
            evTrans.localPosition = Vector3.Lerp(evTrans.localPosition, targetPos, lerpAmount);
            evTrans.localRotation = targetRot;
            evTrans.localScale = targetScl;
        }
    }
}
