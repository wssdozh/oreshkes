using System.Collections;
using UnityEngine;

public class Bullet : Ammo
{
    [Header("Зависимости")]
    [SerializeField] private TrailRenderer _trailRenderer;

    [Header("Настройки")]
    [SerializeField] private float _minDamage = 3f;
    [SerializeField] private float _maxDamage = 6f;
    [SerializeField] private float _impulseStrength = 3f;

    private Coroutine _trailRoutine;

    protected override void OnAmmoEnabled()
    {
        if (_trailRoutine == null == false)
        {
            StopCoroutine(_trailRoutine);
            _trailRoutine = null;
        }

        if (_trailRenderer == null == false)
        {
            _trailRenderer.emitting = false;
            _trailRenderer.Clear();
            _trailRoutine = StartCoroutine(EnableTrailNextFrame());
        }
    }

    private IEnumerator EnableTrailNextFrame()
    {
        yield return null;

        _trailRenderer.Clear();
        _trailRenderer.emitting = true;
        _trailRoutine = null;
    }

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

    protected override void OnLifeEnding()
    {
        if (_trailRoutine == null == false)
        {
            StopCoroutine(_trailRoutine);
            _trailRoutine = null;
        }

        if (_trailRenderer == null == false)
        {
            _trailRenderer.emitting = false;
            _trailRenderer.Clear();
        }
    }
}
