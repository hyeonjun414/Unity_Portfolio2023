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
        
        public void Init()
        {
            canvasRect = canvas.GetComponent<RectTransform>();
            canvas.worldCamera = GameManager.Instance.mainCam;
            canvas.sortingOrder = isFront ? 15 : 10;
            
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
    }
}
