using UnityEngine;

public abstract class AttackShapeBase : ScriptableObject
{
    public abstract Collider[] GetTargets(Transform originTransform, float range, LayerMask hitLayers);
    public abstract void DrawGizmos(Transform originTransform, float range);
}
