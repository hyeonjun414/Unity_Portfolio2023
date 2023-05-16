using System;
using Manager;
using UnityEngine;

namespace View
{
    public class SceneView : MonoBehaviour
    {
        public SceneView Parent;
        public SceneView Child;

        public Canvas canvas;
        public RectTransform canvasRect;

        public bool isFront;


        public void SetParent(SceneView parent)
        {
            Parent = parent;
            if (Parent != null && Parent.isFront == false)
            {
                Parent.gameObject.SetActive(false);
            }
        }

        public void SetChild(SceneView child)
        {
            Child = child;
        }

        public void Init()
        {
            canvasRect = canvas.GetComponent<RectTransform>();
            canvas.worldCamera = GameManager.Instance.mainCam;
            canvas.sortingOrder = isFront ? 15 : 10;
            
        }
    }
}
