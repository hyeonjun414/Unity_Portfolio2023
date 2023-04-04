using Presenter;
using UnityEngine;

namespace View
{
    public class StageNodeView : MonoBehaviour
    {
        public Stage Presenter;


        public void Init(Stage stage)
        {
            Presenter = stage;
        }
    }
}
