using UnityEngine;

public class Rocket : Ammo
{
    [Header("Настройки")]
    [SerializeField] private float _minDamage = 3f;
    [SerializeField] private float _maxDamage = 6f;
    [SerializeField] private float _impulseStrength = 3f;
    [SerializeField] private float _radiusImpulse = 3f;
    [SerializeField] private float _upwardsModifier = 0.5f;

    protected override void OnHitTarget(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radiusImpulse, TargetLayers);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent<Health>(out Health health))
            {
                health.Decrease(Random.Range(_minDamage, _maxDamage));
            }

            Rigidbody rigidbody = collider.attachedRigidbody;

            if (rigidbody == null)
            {
                continue;
            }

            rigidbody.AddExplosionForce(
                _impulseStrength,
                transform.position,
                _radiusImpulse,
                _upwardsModifier,
                ForceMode.Impulse);
        }
    }
}
