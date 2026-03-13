using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyRangedBrain : MonoBehaviour, IEnemyBrain
{
    private const int IdlePointTryCount = 12;
    private const float LookDistance = 2f;
    private const float ZeroThreshold = 0.0001f;

    [Header("Dependencies")]
    [SerializeField] private Enemy _enemy;
    [SerializeField] private TargetVision _targetVision;
    [SerializeField] private EnemyMove _enemyMove;
    [SerializeField] private EnemyRotator _enemyRotator;
    [SerializeField] private EnemyAnimation _animation;
    [SerializeField] private WeaponHolder _weaponHolder;

    [Header("Combat")]
    [SerializeField] private float _fightMin = 2.75f;
    [SerializeField] private float _fightMax = 5.75f;
    [SerializeField] private float _fireMax = 7.25f;
    [SerializeField] private float _fightGap = 0.45f;
    [SerializeField] private float _runStart = 7f;
    [SerializeField] private float _runStop = 5.5f;
    [SerializeField] private float _lostStop = 0.35f;
    [SerializeField] private float _searchWait = 1.35f;
    [SerializeField] private float _flipMin = 0.9f;
    [SerializeField] private float _flipMax = 2f;

    [Header("Idle")]
    [SerializeField] private float _idleMoveMin = 2.5f;
    [SerializeField] private float _idleMoveMax = 4.5f;
    [SerializeField] private float _idleWaitMin = 1.1f;
    [SerializeField] private float _idleWaitMax = 2.2f;
    [SerializeField] private float _idleReach = 0.2f;

    [Header("Steering")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _allyMask = ~0;
    [SerializeField] private float _probeRad = 0.22f;
    [SerializeField] private float _probeHeight = 0.6f;
    [SerializeField] private float _probeDistance = 0.9f;
    [SerializeField] private float _probeAngle = 25f;
    [SerializeField] private float _avoidWeight = 1.05f;
    [SerializeField] private float _separationRad = 1.35f;
    [SerializeField] private float _separationWeight = 2.2f;

    [Header("Combat Move")]
    [SerializeField] private float _orbitWeight = 1f;
    [SerializeField] private float _ringWeight = 1.2f;
    [SerializeField] private float _slotAngle = 18f;
    [SerializeField] private float _slotRad = 3f;
    [SerializeField] private int _slotCount = 3;
    [SerializeField] private float _recoverBack = 1.2f;
    [SerializeField] private float _recoverSide = 0.8f;

    private EnemySteering _enemySteering;
    private System.Random _random;
    private EnemyState _state;
    private Vector3 _lastSeenPoint;
    private Vector3 _lastSeenDirection;
    private Vector3 _idlePoint;
    private Vector3 _idleLookPoint;
    private bool _hasLastSeenPoint;
    private bool _isIdleWalk;
    private bool _isClockwise;
    private float _idleTimer;
    private float _searchTimer;
    private EnemyRoomLock _enemyRoomLock;

    public EnemyState State => _state;

    private void Awake()
    {
        ResolveDependencies();

        if (_enemy == null)
        {
            throw new InvalidOperationException(nameof(_enemy));
        }

        if (_targetVision == null)
        {
            throw new InvalidOperationException(nameof(_targetVision));
        }

        if (_enemyMove == null)
        {
            throw new InvalidOperationException(nameof(_enemyMove));
        }

        if (_enemyRotator == null)
        {
            throw new InvalidOperationException(nameof(_enemyRotator));
        }

        if (_animation == null)
        {
            throw new InvalidOperationException(nameof(_animation));
        }

        if (_weaponHolder == null)
        {
            throw new InvalidOperationException(nameof(_weaponHolder));
        }

        ValidateSettings();

        _random = new System.Random(GetSeed());
        _enemySteering = new EnemySteering(transform, _enemyMove, _enemyRotator);
        _enemySteering.SetObstacle(_obstacleMask, _probeRad, _probeHeight, _probeDistance, _probeAngle, _avoidWeight);
        _enemySteering.SetSpacing(_allyMask, _separationRad, _separationWeight);
    }

    private void ResolveDependencies()
    {
        if (_enemy == null)
        {
            _enemy = GetComponent<Enemy>();
        }

        if (_targetVision == null)
        {
            _targetVision = GetComponentInChildren<TargetVision>();
        }

        if (_enemyMove == null)
        {
            _enemyMove = GetComponentInChildren<EnemyMove>();
        }

        if (_enemyRotator == null)
        {
            _enemyRotator = GetComponentInChildren<EnemyRotator>();
        }

        if (_animation == null)
        {
            _animation = GetComponentInChildren<EnemyAnimation>();
        }

        if (_weaponHolder == null)
        {
            _weaponHolder = GetComponentInChildren<WeaponHolder>();
        }
    }

    private void OnEnable()
    {
        _enemy.Died += OnDied;
        ResetState();
    }

    private void OnDisable()
    {
        _enemy.Died -= OnDied;
        StopCombat();
        _enemyMove.SetRun(false);
        _enemySteering.Stop();
    }

    private void FixedUpdate()
    {
        if (_enemy.IsDead)
        {
            return;
        }

        RefreshRoomLock();
        _targetVision.Refresh();

        Transform currentTarget = GetVisibleTarget();

        if (_enemySteering.ResolveOverlap())
        {
            RotateTarget(currentTarget);

            return;
        }

        if (currentTarget != null)
        {
            UpdateLastSeen(currentTarget);
            ProcessVisible(currentTarget);

            return;
        }

        ProcessHidden();
    }

    private void ValidateSettings()
    {
        if (_fightMin <= 0f)
        {
            throw new InvalidOperationException(nameof(_fightMin));
        }

        if (_fightMax < _fightMin)
        {
            throw new InvalidOperationException(nameof(_fightMax));
        }

        if (_fireMax < _fightMax)
        {
            throw new InvalidOperationException(nameof(_fireMax));
        }

        if (_fightGap <= 0f)
        {
            throw new InvalidOperationException(nameof(_fightGap));
        }

        if (_runStart <= 0f)
        {
            throw new InvalidOperationException(nameof(_runStart));
        }

        if (_runStop <= 0f)
        {
            throw new InvalidOperationException(nameof(_runStop));
        }

        if (_runStart < _runStop)
        {
            throw new InvalidOperationException(nameof(_runStart));
        }

        if (_lostStop <= 0f)
        {
            throw new InvalidOperationException(nameof(_lostStop));
        }

        if (_searchWait < 0f)
        {
            throw new InvalidOperationException(nameof(_searchWait));
        }

        if (_flipMin <= 0f)
        {
            throw new InvalidOperationException(nameof(_flipMin));
        }

        if (_flipMax < _flipMin)
        {
            throw new InvalidOperationException(nameof(_flipMax));
        }

        if (_idleMoveMin <= 0f)
        {
            throw new InvalidOperationException(nameof(_idleMoveMin));
        }

        if (_idleMoveMax < _idleMoveMin)
        {
            throw new InvalidOperationException(nameof(_idleMoveMax));
        }

        if (_idleWaitMin < 0f)
        {
            throw new InvalidOperationException(nameof(_idleWaitMin));
        }

        if (_idleWaitMax < _idleWaitMin)
        {
            throw new InvalidOperationException(nameof(_idleWaitMax));
        }

        if (_idleReach <= 0f)
        {
            throw new InvalidOperationException(nameof(_idleReach));
        }

        if (_probeRad <= 0f)
        {
            throw new InvalidOperationException(nameof(_probeRad));
        }

        if (_probeHeight <= 0f)
        {
            throw new InvalidOperationException(nameof(_probeHeight));
        }

        if (_probeDistance <= 0f)
        {
            throw new InvalidOperationException(nameof(_probeDistance));
        }

        if (_probeAngle <= 0f)
        {
            throw new InvalidOperationException(nameof(_probeAngle));
        }

        if (_avoidWeight < 0f)
        {
            throw new InvalidOperationException(nameof(_avoidWeight));
        }

        if (_separationRad <= 0f)
        {
            throw new InvalidOperationException(nameof(_separationRad));
        }

        if (_separationWeight < 0f)
        {
            throw new InvalidOperationException(nameof(_separationWeight));
        }

        if (_orbitWeight < 0f)
        {
            throw new InvalidOperationException(nameof(_orbitWeight));
        }

        if (_ringWeight < 0f)
        {
            throw new InvalidOperationException(nameof(_ringWeight));
        }

        if (_slotAngle < 0f)
        {
            throw new InvalidOperationException(nameof(_slotAngle));
        }

        if (_slotRad <= 0f)
        {
            throw new InvalidOperationException(nameof(_slotRad));
        }

        if (_slotCount <= 0)
        {
            throw new InvalidOperationException(nameof(_slotCount));
        }

        if (_recoverBack < 0f)
        {
            throw new InvalidOperationException(nameof(_recoverBack));
        }

        if (_recoverSide < 0f)
        {
            throw new InvalidOperationException(nameof(_recoverSide));
        }
    }

    private void ProcessVisible(Transform currentTarget)
    {
        Vector3 currentPoint = GetFlatPoint(transform.position);
        Vector3 targetPoint = GetTargetPoint(currentTarget);
        float distance = Vector3.Distance(currentPoint, targetPoint);
        FireExecutor fireExecutor = GetFireExecutor();
        bool hasFireLine = HasFireLine(targetPoint);

        if (fireExecutor != null)
        {
            fireExecutor.SetAimPoint(targetPoint);
        }

        if (distance > _fightMax)
        {
            ProcessChase(targetPoint, distance);
        }

        else if (distance < _fightMin)
        {
            ProcessRetreat(currentPoint, targetPoint);
        }

        else
        {
            _state = EnemyState.Fight;
            _enemyMove.SetRun(false);

            if (hasFireLine)
            {
                _enemySteering.Stop();
                _enemyRotator.RotateToPoint(targetPoint);
            }

            else
            {
                if (_enemySteering.MoveToPoint(targetPoint, _fightMin, targetPoint) == false)
                {
                    _enemySteering.LookToPoint(targetPoint);
                }
            }
        }

        TryShoot(fireExecutor, targetPoint, distance, hasFireLine);
    }

    private void ProcessHidden()
    {
        ClearAim();

        if (_hasLastSeenPoint)
        {
            ProcessSearch();

            return;
        }

        ProcessIdle();
    }

    private void ProcessSearch()
    {
        Vector3 currentPoint = GetFlatPoint(transform.position);
        float distance = Vector3.Distance(currentPoint, _lastSeenPoint);

        if (distance > _lostStop)
        {
            _state = EnemyState.Search;
            _enemyMove.SetRun(IsRunNeeded(distance));

            if (_enemySteering.MoveToPoint(_lastSeenPoint, _lostStop, _lastSeenPoint))
            {
                return;
            }
        }

        _enemyMove.SetRun(false);
        _state = EnemyState.Search;
        _enemySteering.LookToPoint(_lastSeenPoint + (_lastSeenDirection * LookDistance));
        _searchTimer -= Time.fixedDeltaTime;

        if (_searchTimer > 0f)
        {
            return;
        }

        _hasLastSeenPoint = false;
        StartIdleLook();
    }

    private void ProcessIdle()
    {
        if (_isIdleWalk)
        {
            _state = EnemyState.Patrol;
            ProcessIdleWalk();

            return;
        }

        _state = EnemyState.Watch;
        ProcessIdleLook();
    }

    private void ProcessIdleWalk()
    {
        Vector3 currentPoint = GetFlatPoint(transform.position);
        float distance = Vector3.Distance(currentPoint, _idlePoint);
        _enemyMove.SetRun(false);

        if (distance <= _idleReach)
        {
            StartIdleLook();

            return;
        }

        if (_enemySteering.MoveToPoint(_idlePoint, _idleReach, _idlePoint))
        {
            return;
        }

        StartIdleLook();
    }

    private void ProcessIdleLook()
    {
        _enemyMove.SetRun(false);
        _enemySteering.LookToPoint(_idleLookPoint);
        _idleTimer -= Time.fixedDeltaTime;

        if (_idleTimer > 0f)
        {
            return;
        }

        StartIdleWalk();
    }

    private void StartIdleWalk()
    {
        Vector3 currentPoint = GetFlatPoint(transform.position);
        int tryIndex = 0;

        while (tryIndex < IdlePointTryCount)
        {
            Vector3 direction = GetRandomDirection();
            float distance = GetRandomRange(_idleMoveMin, _idleMoveMax);
            Vector3 candidatePoint = currentPoint + (direction * distance);
            candidatePoint.y = transform.position.y;

            Vector3 safePoint = _enemySteering.GetSafePoint(candidatePoint, _probeRad);
            float safeDistance = Vector3.Distance(currentPoint, safePoint);

            if (safeDistance <= _idleReach)
            {
                tryIndex += 1;

                continue;
            }

            if (_enemySteering.HasPointClearance(safePoint) == false)
            {
                tryIndex += 1;

                continue;
            }

            _idlePoint = safePoint;
            _isIdleWalk = true;
            _idleTimer = 0f;

            return;
        }

        StartIdleLook();
    }

    private void StartIdleLook()
    {
        _isIdleWalk = false;
        _idleTimer = GetRandomRange(_idleWaitMin, _idleWaitMax);
        _idleLookPoint = transform.position + (GetRandomDirection() * LookDistance);
        _enemySteering.Stop();
    }

    private void ProcessChase(Vector3 targetPoint, float distance)
    {
        _state = EnemyState.Chase;
        _enemyMove.SetRun(IsRunNeeded(distance));

        if (_enemySteering.MoveToPoint(targetPoint, _fightMax, targetPoint))
        {
            return;
        }

        _enemyMove.SetRun(false);
        _enemySteering.LookToPoint(targetPoint);
    }

    private void ProcessRetreat(Vector3 currentPoint, Vector3 targetPoint)
    {
        _state = EnemyState.Fight;
        _enemyMove.SetRun(false);

        Vector3 retreatPoint = GetRetreatPoint(currentPoint, targetPoint);

        if (_enemySteering.MoveToPoint(retreatPoint, _fightGap, targetPoint))
        {
            return;
        }

        _enemySteering.LookToPoint(targetPoint);
    }

    private void TryShoot(FireExecutor fireExecutor, Vector3 targetPoint, float distance, bool hasFireLine)
    {
        if (fireExecutor == null)
        {
            return;
        }

        if (distance > _fireMax)
        {
            return;
        }

        if (hasFireLine == false)
        {
            return;
        }

        fireExecutor.SetAimPoint(targetPoint);

        if (fireExecutor.TryFire() == false)
        {
            return;
        }
    }

    private void UpdateLastSeen(Transform currentTarget)
    {
        Vector3 currentPoint = GetFlatPoint(transform.position);
        Vector3 targetPoint = GetTargetPoint(currentTarget);
        Vector3 lastDirection = targetPoint - currentPoint;
        lastDirection.y = 0f;

        if (lastDirection.sqrMagnitude > ZeroThreshold)
        {
            lastDirection.Normalize();
            _lastSeenDirection = lastDirection;
        }

        _lastSeenPoint = targetPoint;
        _hasLastSeenPoint = true;
        _searchTimer = _searchWait;
    }

    private void ResetState()
    {
        _state = EnemyState.Watch;
        _hasLastSeenPoint = false;
        _lastSeenDirection = GetRandomDirection();
        _searchTimer = 0f;
        _isClockwise = GetRandomBool();
        _enemyMove.SetRun(false);
        _enemySteering.Stop();
        _enemyRotator.SnapToDirection(_lastSeenDirection);
        StartIdleWalk();
    }

    private void StopCombat()
    {
        FireExecutor fireExecutor = GetFireExecutor();

        if (fireExecutor == null)
        {
            return;
        }

        fireExecutor.StopFiring();
        fireExecutor.ClearAimPoint();
    }

    private void ClearAim()
    {
        FireExecutor fireExecutor = GetFireExecutor();

        if (fireExecutor == null)
        {
            return;
        }

        fireExecutor.ClearAimPoint();
    }

    private void OnDied()
    {
        StopCombat();
        _enemyMove.SetRun(false);
        _enemySteering.Stop();
        enabled = false;
    }

    private void RefreshRoomLock()
    {
        EnemyRoomLock enemyRoomLock = GetEnemyRoomLock();
        _enemySteering.SetRoomLock(enemyRoomLock);
    }

    private Transform GetVisibleTarget()
    {
        if (_targetVision.IsTargetVisible == false)
        {
            return null;
        }

        Transform currentTarget = _targetVision.CurrentTarget;

        if (currentTarget == null)
        {
            return null;
        }

        EnemyRoomLock enemyRoomLock = GetEnemyRoomLock();

        if (enemyRoomLock == null)
        {
            return currentTarget;
        }

        Vector3 targetPoint = GetTargetPoint(currentTarget);

        if (enemyRoomLock.ContainsRoomPoint(currentTarget.position) == false && enemyRoomLock.ContainsRoomPoint(targetPoint) == false)
        {
            return null;
        }

        return currentTarget;
    }

    private Vector3 GetTargetPoint(Transform currentTarget)
    {
        if (currentTarget == null)
        {
            return GetFlatPoint(transform.position);
        }

        if (currentTarget == _targetVision.CurrentTarget)
        {
            return GetFlatPoint(_targetVision.CurrentTargetPoint);
        }

        return GetFlatPoint(currentTarget.position);
    }

    private FireExecutor GetFireExecutor()
    {
        return _weaponHolder.FireExecutor;
    }

    private EnemyRoomLock GetEnemyRoomLock()
    {
        if (_enemyRoomLock != null)
        {
            return _enemyRoomLock;
        }

        _enemyRoomLock = GetComponent<EnemyRoomLock>();

        return _enemyRoomLock;
    }

    private void RotateTarget(Transform currentTarget)
    {
        if (currentTarget == null)
        {
            return;
        }

        _enemyRotator.RotateToPoint(GetTargetPoint(currentTarget));
    }

    private bool HasFireLine(Vector3 targetPoint)
    {
        if (_enemySteering.IsLineBlocked(targetPoint))
        {
            return false;
        }

        return true;
    }

    private bool IsRunNeeded(float distance)
    {
        if (_enemyMove.IsRunRequested)
        {
            if (distance > _runStop)
            {
                return true;
            }

            return false;
        }

        if (distance >= _runStart)
        {
            return true;
        }

        return false;
    }

    private float GetRandomRange(float minValue, float maxValue)
    {
        if (maxValue - minValue <= ZeroThreshold)
        {
            return minValue;
        }

        return Mathf.Lerp(minValue, maxValue, Next01());
    }

    private Vector3 GetRandomDirection()
    {
        float angle = GetRandomRange(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 direction = rotation * Vector3.forward;
        direction.y = 0f;

        if (direction.sqrMagnitude <= ZeroThreshold)
        {
            return Vector3.forward;
        }

        direction.Normalize();

        return direction;
    }

    private bool GetRandomBool()
    {
        if (Next01() >= 0.5f)
        {
            return true;
        }

        return false;
    }

    private float Next01()
    {
        if (_random == null)
        {
            return 0.5f;
        }

        return (float)_random.NextDouble();
    }

    private int GetSeed()
    {
        int seed = gameObject.GetInstanceID();

        if (seed == int.MinValue)
        {
            return int.MaxValue;
        }

        if (seed < 0)
        {
            seed = -seed;
        }

        return seed;
    }

    private Vector3 GetRetreatPoint(Vector3 currentPoint, Vector3 targetPoint)
    {
        Vector3 retreatDirection = currentPoint - targetPoint;
        retreatDirection.y = 0f;

        if (retreatDirection.sqrMagnitude <= ZeroThreshold)
        {
            retreatDirection = -_enemyRotator.ForwardDirection;
        }

        if (retreatDirection.sqrMagnitude > 1f)
        {
            retreatDirection.Normalize();
        }

        Vector3 sideDirection = Vector3.Cross(Vector3.up, retreatDirection);

        if (_isClockwise == false)
        {
            sideDirection = -sideDirection;
        }

        float retreatDistance = _fightMin - Vector3.Distance(currentPoint, targetPoint);

        if (retreatDistance < _fightGap)
        {
            retreatDistance = _fightGap;
        }

        Vector3 retreatPoint = currentPoint + (retreatDirection * retreatDistance);
        retreatPoint += sideDirection * (_recoverSide * 0.25f);
        retreatPoint.y = transform.position.y;

        return retreatPoint;
    }

    private Vector3 GetFlatPoint(Vector3 point)
    {
        point.y = transform.position.y;

        return point;
    }
}
