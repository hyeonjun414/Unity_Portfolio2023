using UnityEngine;
using View;

public class AnimationEventHandler : MonoBehaviour
{
    void Attack() {}

    void RemoveFromStage()
    {
        var entity = GetComponentInParent<EntityView>();
        entity.RemoveFromStage();
    }
}
