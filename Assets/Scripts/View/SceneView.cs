using UnityEngine;

namespace View
{
    public class SceneView : MonoBehaviour
    {
        public SceneView Parent;
        public SceneView Child;

        public void SetParent(SceneView parent)
        {
            Parent = parent;
            if (Parent != null)
            {
                Parent.gameObject.SetActive(false);
            }
        }

        public void SetChild(SceneView child)
        {
            Child = child;
        }
    }
}
