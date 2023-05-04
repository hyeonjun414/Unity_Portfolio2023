using Presenter;
using UnityEngine;

namespace View
{
    public class ActionBar : MonoBehaviour
    {
        public Transform start;
        public Transform end;

        public ApView enemyApViewPrefab;
        public ApView allyApViewPrefab;

        public void AddEntity(Character character)
        {
            ApView prefab = null;
            switch (character)
            {
                case Hero:
                case Ally:
                    prefab = allyApViewPrefab;
                    break;
                case Enemy:
                    prefab = enemyApViewPrefab;
                    break;
            }
            var inst = Instantiate(prefab, transform);
            inst.Init(character, start, end);
        }
    }
}
