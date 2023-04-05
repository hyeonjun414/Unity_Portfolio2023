using UnityEngine;

namespace View
{
    public class MapPathView : MonoBehaviour
    {
        public RectTransform viewRect;
        public void SetPath(MapNodeView start, MapNodeView end)
        {
            transform.position = start.transform.position;
            var startPos = transform.position;
            var endPos = end.transform.position;
            
            var dist = Vector3.Distance(startPos, endPos);
            var angle = Mathf.Atan2((endPos - startPos).y, (endPos - startPos).x) * Mathf.Rad2Deg;
            viewRect.sizeDelta = new Vector2(30, dist);
            viewRect.rotation = Quaternion.Euler(0,0,angle - 90);
            
        }
    }
}
