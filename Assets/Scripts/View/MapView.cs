using System.Collections.Generic;
using Manager;
using Presenter;
using UnityEngine;

namespace View
{
    public class MapView : MonoBehaviour
    {
        public Map Presenter;

        public void Start()
        {
            if (GameManager.Instance == null)
                return;
            
            Presenter = GameManager.Instance.CurMap;
            Presenter.View = this;
            Presenter.Init();
        }
    }
}
