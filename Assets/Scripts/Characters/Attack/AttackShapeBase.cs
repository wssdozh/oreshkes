using UnityEngine;

public abstract class AttackShapeBase : ScriptableObject
{
    public int GetTargets(Transform originTransform, float range, LayerMask hitLayers, Collider[] resultBuffer)
    {
        return GetTargets(originTransform.position, originTransform.forward, range, hitLayers, resultBuffer);
    }

    public void DrawGizmos(Transform originTransform, float range)
    {
        DrawGizmos(originTransform.position, originTransform.forward, range);
    }

    public abstract int GetTargets(Vector3 originPoint, Vector3 attackDirection, float range, LayerMask hitLayers, Collider[] resultBuffer);
    public abstract void DrawGizmos(Vector3 originPoint, Vector3 attackDirection, float range);
}
