using System;
using System.Collections;
using UnityEngine;

public abstract class FireExecutor : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _fireRatePerSecond = 5f;
    [SerializeField] private LayerMask _targetLayers;
    [SerializeField] private float _maxAimAngleDegrees = 30f;

    private Coroutine _firingCoroutine;
    private bool _isFiring;
    private float _nextShotTime;
    private float _lastShotSecondsPerShot;

    protected float FireRatePerSecond => _fireRatePerSecond;
    protected LayerMask TargetLayers => _targetLayers;

    protected bool HasAimPoint { get; private set; }
    protected Vector3 AimPoint { get; private set; }

    protected virtual void Awake()
    {
        if (_fireRatePerSecond <= 0f)
        {
            throw new InvalidOperationException(nameof(_fireRatePerSecond));
        }

        if (_maxAimAngleDegrees <= 0f)
        {
            throw new InvalidOperationException(nameof(_maxAimAngleDegrees));
        }
    }

    protected virtual void OnEnable()
    {
        _nextShotTime = 0f;
        _lastShotSecondsPerShot = 0f;
    }

    public float GetFireCooldown01()
    {
        if (Time.time >= _nextShotTime)
        {
            return 1f;
        }

        float remainingSeconds = _nextShotTime - Time.time;
        float cooldown01 = 1f - (remainingSeconds / _lastShotSecondsPerShot);

        return Mathf.Clamp01(cooldown01);
    }

    public void SetAimPoint(Vector3 aimPoint)
    {
        AimPoint = aimPoint;
        HasAimPoint = true;
    }

    public void ClearAimPoint()
    {
        HasAimPoint = false;
    }

    public bool TryStartFiring()
    {
        if (_isFiring == true)
        {
            return false;
        }

        _isFiring = true;
        _firingCoroutine = StartCoroutine(FiringCoroutine());

        return true;
    }

    public void StartFiring()
    {
        TryStartFiring();
    }

    public void StopFiring()
    {
        if (_isFiring == false)
        {
            return;
        }

        _isFiring = false;

        if (_firingCoroutine != null)
        {
            StopCoroutine(_firingCoroutine);
            _firingCoroutine = null;
        }
    }

    public void SetTargetLayers(LayerMask targetLayers)
    {
        _targetLayers = targetLayers;
    }

    public bool TryFire()
    {
        if (Time.time < _nextShotTime)
        {
            return false;
        }

        float secondsPerShot = 1f / _fireRatePerSecond;

        _lastShotSecondsPerShot = secondsPerShot;
        _nextShotTime = Time.time + secondsPerShot;

        return TryFireInternal();
    }

    protected abstract bool TryFireInternal();

    protected void RotateMuzzleToAimPoint(Transform muzzle)
    {
        if (HasAimPoint == false)
        {
            return;
        }

        Vector3 desiredDirection = AimPoint - muzzle.position;

        if (desiredDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Vector3 upAxis = transform.up;

        Vector3 flatDirection = Vector3.ProjectOnPlane(desiredDirection, upAxis);

        if (flatDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion yawRotation = Quaternion.LookRotation(flatDirection, upAxis);

        Vector3 rightAxis = yawRotation * Vector3.right;

        float pitchDegrees = Vector3.SignedAngle(flatDirection, desiredDirection, rightAxis);

        float clampedPitchDegrees = Mathf.Clamp(pitchDegrees, -_maxAimAngleDegrees, _maxAimAngleDegrees);

        Quaternion pitchRotation = Quaternion.AngleAxis(clampedPitchDegrees, Vector3.right);

        muzzle.rotation = yawRotation * pitchRotation;
    }

    private IEnumerator FiringCoroutine()
    {
        float secondsPerShot = 1f / _fireRatePerSecond;
        WaitForSeconds wait = new WaitForSeconds(secondsPerShot);

        while (_isFiring == true)
        {
            TryFire();

            yield return wait;
        }

        _firingCoroutine = null;
    }
}
