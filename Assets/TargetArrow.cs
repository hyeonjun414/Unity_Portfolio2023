using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core;
using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    public List<GameObject> arrows;
    public Transform start, mid, end;

    public void ActiveArrow(Transform start)
    {
        this.start = start;
        mid.transform.position = start.position + Vector3.up * 700f;
        end.position = Input.mousePosition;
        PlaceObjectOnBezierCurve();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        end.position = Input.mousePosition;
        PlaceObjectOnBezierCurve();
    }

    private void PlaceObjectOnBezierCurve()
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            var t = i / (float)(arrows.Count - 1);
            var oneMinusT = 1 - t;
            var oneMinusTSquared = oneMinusT * oneMinusT;

            var pos = oneMinusTSquared * start.position + 2 * oneMinusT * t * mid.position + t * t * end.position;
            arrows[i].transform.position = pos;
            arrows[i].transform.localScale = Vector3.Lerp(Vector3.one * 0.3f, Vector3.one * 0.7f, t);
        }

        for (int i = 0; i < arrows.Count - 1; i++)
        {
            Vector3 dir = arrows[i + 1].transform.localPosition - arrows[i].transform.localPosition;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle -90));
        }
    }
}
