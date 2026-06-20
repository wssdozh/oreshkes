using DG.Tweening;
using UnityEngine;

public sealed class PickupIdleMotion : PickupIdleBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _moveAmplitude = 0.15f;
    [SerializeField] private float _moveDurationSeconds = 1.2f;

    [SerializeField] private Vector3 _rotationDegrees = new Vector3(0f, 35f, 0f);
    [SerializeField] private float _rotationDurationSeconds = 1.6f;

    [SerializeField] private float _randomStartDelaySeconds = 0.25f;

    [Header("Высота")]
    [SerializeField] private float _baseHeightOffset = 0.35f;
    [SerializeField] private float _baseOffsetTransitionSeconds = 0.2f;

    private Tween _startDelayTween;
    private Tween _baseOffsetTween;
    private Tween _moveTween;
    private Tween _rotationTween;

    private Vector3 _defaultLocalPosition;
    private Vector3 _defaultLocalRotation;

    private Vector3 _baseLocalPosition;

    private void Awake()
    {
        _defaultLocalPosition = transform.localPosition;
        _defaultLocalRotation = transform.localEulerAngles;
    }

    protected override void OnIdleActivated()
    {
        StartMotion();
    }

    protected override void OnIdleDeactivated()
    {
        StopMotion();
    }

    private void StartMotion()
    {
        StopMotion();

        _baseLocalPosition = _defaultLocalPosition;

        if (Mathf.Abs(_baseHeightOffset) > Mathf.Epsilon)
            _baseLocalPosition = new Vector3(_baseLocalPosition.x, _baseLocalPosition.y + _baseHeightOffset, _baseLocalPosition.z);

        float startDelaySeconds = 0f;

        if (_randomStartDelaySeconds > 0f)
            startDelaySeconds = Random.Range(0f, _randomStartDelaySeconds);

        _startDelayTween = DOVirtual.DelayedCall(startDelaySeconds, StartLoopTweens);
        _startDelayTween.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        _startDelayTween.OnKill(OnStartDelayTweenKilled);
    }

    private void StopMotion()
    {
        _startDelayTween = KillTween(_startDelayTween);
        _baseOffsetTween = KillTween(_baseOffsetTween);
        _moveTween = KillTween(_moveTween);
        _rotationTween = KillTween(_rotationTween);

        transform.localPosition = _defaultLocalPosition;
        transform.localEulerAngles = _defaultLocalRotation;
    }

    private void StartLoopTweens()
    {
        transform.localPosition = _defaultLocalPosition;
        transform.localEulerAngles = _defaultLocalRotation;

        if (_baseOffsetTransitionSeconds > 0f)
        {
            _baseOffsetTween = transform.DOLocalMove(_baseLocalPosition, _baseOffsetTransitionSeconds);
            _baseOffsetTween.SetEase(Ease.OutSine);
            _baseOffsetTween.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _baseOffsetTween.OnComplete(StartLoopTweensAfterBaseReached);
            _baseOffsetTween.OnKill(OnBaseOffsetTweenKilled);

            return;
        }

        transform.localPosition = _baseLocalPosition;

        StartLoopTweensAfterBaseReached();
    }

    private void StartLoopTweensAfterBaseReached()
    {
        Vector3 targetLocalPosition = _baseLocalPosition + Vector3.up * _moveAmplitude;
        Vector3 targetLocalRotation = _defaultLocalRotation + _rotationDegrees;

        if (Mathf.Abs(_moveAmplitude) > Mathf.Epsilon)
        {
            _moveTween = transform.DOLocalMove(targetLocalPosition, _moveDurationSeconds);
            _moveTween.SetEase(Ease.InOutSine);
            _moveTween.SetLoops(-1, LoopType.Yoyo);
            _moveTween.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _moveTween.OnKill(OnMoveTweenKilled);
        }

        if (_rotationDegrees.sqrMagnitude > Mathf.Epsilon)
        {
            _rotationTween = transform.DOLocalRotate(targetLocalRotation, _rotationDurationSeconds, RotateMode.FastBeyond360);
            _rotationTween.SetEase(Ease.InOutSine);
            _rotationTween.SetLoops(-1, LoopType.Yoyo);
            _rotationTween.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _rotationTween.OnKill(OnRotationTweenKilled);
        }
    }

    private Tween KillTween(Tween tween)
    {
        if (tween == null)
            return null;

        if (tween.IsActive())
            tween.Kill(false);

        return null;
    }

    private void OnStartDelayTweenKilled()
    {
        _startDelayTween = null;
    }

    private void OnBaseOffsetTweenKilled()
    {
        _baseOffsetTween = null;
    }

    private void OnMoveTweenKilled()
    {
        _moveTween = null;
    }

    private void OnRotationTweenKilled()
    {
        _rotationTween = null;
    }
}
