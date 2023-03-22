using UnityEditor.Animations;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "ScriptableData/EnemyData", fileName = "EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public AnimatorOverrideController enemyAnim;
        public bool isAlreadyLeft;
    }
}
