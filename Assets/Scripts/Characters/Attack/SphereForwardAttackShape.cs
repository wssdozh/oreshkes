using UnityEngine;

[CreateAssetMenu(fileName = "SphereForwardAttackShape", menuName = "Combat/Attack Shapes/Sphere Forward")]
public class SphereForwardAttackShape : AttackShapeBase
{
    public override Collider[] GetTargets(Transform originTransform, float range, LayerMask hitLayers)
    {
        Vector3 center = originTransform.position + originTransform.forward * (range * 0.5f);
        Collider[] hits = Physics.OverlapSphere(center, range, hitLayers);

        return hits;
    }

    public override void DrawGizmos(Transform originTransform, float range)
    {
        Vector3 center = originTransform.position + originTransform.forward * (range * 0.5f);
        Gizmos.DrawWireSphere(center, range);
    }
}
