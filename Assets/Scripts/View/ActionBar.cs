using Presenter;
using UnityEngine;

namespace View
{
    public class ActionBar : MonoBehaviour
    {
        public Transform start;
        public Transform end;

        public ApView apviewPrefab;

        public void AddEntity(Entity entity)
        {
            var inst = Instantiate(apviewPrefab, transform);
            inst.Init(entity, start, end);
        }
    }
}
