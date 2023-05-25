using System;
using Manager;
using Presenter;
using UnityEngine;

namespace View
{
    public class SceneView : MonoBehaviour
    {
        public Scene Presenter;
        
        public Canvas canvas;
        public RectTransform canvasRect;

        public bool isFront;
        public bool isModel;

        public void Init()
        {
            canvasRect = canvas.GetComponent<RectTransform>();
            canvas.worldCamera = GameManager.Instance.mainCam;
        }

        public void SceneViewActive(bool isActive)
        {
            if (isFront == false)
            {
                gameObject.SetActive(isActive);
            }
        }

        public void DestroyScene()
        {
            Destroy(gameObject);
        }

        public void SetLayerOrder(int orderNum)
        {
            canvas.sortingOrder = orderNum;
            if (isFront)
                canvas.sortingLayerName = "Front";
            else if (isModel)
                canvas.sortingLayerName = "Modal";
            else
                canvas.sortingLayerName = "Normal";
        }
    }
}
