using UnityEngine;

public class Bullet : Ammo
{
    [Header("Настройки")]
    [SerializeField] private float _minDamage = 3f;
    [SerializeField] private float _maxDamage = 6f;
    [SerializeField] private float _impulseStrength = 3f;

    protected override void OnHitTarget(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out Health health))
        {
            health.Decrease(Random.Range(_minDamage, _maxDamage));
        }

        if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.AddForce(transform.forward * _impulseStrength, ForceMode.Impulse);
        }
    }
}
