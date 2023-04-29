using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;

public class TargetArrow : MonoBehaviour
{
    public Transform start, mid, end;

    [SerializeField] private List<RectTransform> points;
    [SerializeField] private List<RectTransform> arrows;

    [Header("스케일")] 
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale; 
    [Header("색상")] 
    [SerializeField] private Color baseColor;
    [SerializeField] private Color highlightColor;

    private RectTransform rectTransform;
    private Camera mainCam;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, Input.mousePosition,
            mainCam, out var mousePos);

        points[2].anchoredPosition = mousePos;

        DrawBezier();
        ReScale();
    }

    public void SetStartPoint(Vector3 worldPos)
    {
        points[0].position = worldPos;
    }

    public void SetHighLight(bool enable)
    {
        var color = enable ? highlightColor : baseColor;

        // foreach (var arrow in arrows)
        // {
        //     arrow.GetComponent<Image>().color = color;
        // }
    }

    private Vector2 CurvePoint(float t)
    {
        if (points.Count < 3) return Vector2.zero;

        var s = 1 - t;
        var p0 = Mathf.Pow(s, 2) * points[0].anchoredPosition;
        var p1 = (2 * t * s) * points[1].anchoredPosition;
        var p2 = Mathf.Pow(t, 2) * points[2].anchoredPosition;

        return p0 + p1 + p2;
    }

    private void DrawBezier()
    {
        if (arrows.Count < 2) return;

        var reverse = 1f / (arrows.Count - 1);

        for (var i = 0; i < arrows.Count; i++)
        {
            arrows[i].anchoredPosition = CurvePoint(i * reverse);

            if (i > 0)
            {
                var rot = (arrows[i].anchoredPosition - arrows[i - 1].anchoredPosition).normalized;

                arrows[i].localRotation = Quaternion.FromToRotation(Vector3.right, rot);
            }
        }

        for (int i = 0; i < arrows.Count - 1; i++)
        {
            Vector3 dir = arrows[i + 1].transform.localPosition - arrows[i].transform.localPosition;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle -90));
        }
    }

    private void ReScale()
    {
        if (arrows.Count < 1) return;

        var interval = (maxScale - minScale) / arrows.Count;

        for (var i = 0; i < arrows.Count; i++)
            arrows[i].localScale = Vector3.one * (minScale + interval * i);
    }

    public void ActiveArrow(Transform start)
    {
        // this.start = start;
        // mid.transform.position = start.position + Vector3.up * 700f;
        // end.position = Input.mousePosition;
        // PlaceObjectOnBezierCurve();
        // gameObject.SetActive(true);
    }

    // private void Update()
    // {
    //     end.position = Input.mousePosition;
    //     PlaceObjectOnBezierCurve();
    // }

    // private void PlaceObjectOnBezierCurve()
    // {
    //     for (int i = 0; i < arrows.Count; i++)
    //     {
    //         var t = i / (float)(arrows.Count - 1);
    //         var oneMinusT = 1 - t;
    //         var oneMinusTSquared = oneMinusT * oneMinusT;
    //
    //         var pos = oneMinusTSquared * start.position + 2 * oneMinusT * t * mid.position + t * t * end.position;
    //         arrows[i].transform.position = pos;
    //         arrows[i].transform.localScale = Vector3.Lerp(Vector3.one * 0.3f, Vector3.one * 0.7f, t);
    //     }
    //
    //     for (int i = 0; i < arrows.Count - 1; i++)
    //     {
    //         Vector3 dir = arrows[i + 1].transform.localPosition - arrows[i].transform.localPosition;
    //         float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    //         arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle -90));
    //     }
    // }
}
