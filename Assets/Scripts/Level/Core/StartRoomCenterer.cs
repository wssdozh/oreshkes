using System;
using UnityEngine;

public sealed class StartRoomCenterer : MonoBehaviour
{
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private Transform _playerRoot;
    [SerializeField] private Transform _playerBody;
    [SerializeField, Min(0f)] private float _bodyHeight = 1.5f;
    [SerializeField, Min(1)] private int _syncFrames = 3;

    private Vector3 _targetBodyPosition;
    private int _syncFramesLeft;

    private void Awake()
    {
        if (_levelGenerator == null)
        {
            _levelGenerator = GetComponent<LevelGenerator>();
        }

        if (_levelGenerator == null)
        {
            throw new InvalidOperationException(nameof(_levelGenerator));
        }

        if (_playerRoot == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null)
            {
                throw new InvalidOperationException(nameof(playerObject));
            }

            _playerRoot = playerObject.transform;
        }

        if (_playerBody == null)
        {
            _playerBody = _playerRoot;
        }
    }

    private void OnEnable()
    {
        _levelGenerator.GenerationCompleted += OnGenerationCompleted;
    }

    private void LateUpdate()
    {
        if (_syncFramesLeft <= 0)
        {
            return;
        }

        CenterPlayerBody();

        _syncFramesLeft = _syncFramesLeft - 1;
    }

    private void OnDisable()
    {
        _levelGenerator.GenerationCompleted -= OnGenerationCompleted;
    }

    private void CenterPlayerBody()
    {
        Vector3 rootToBodyOffset = _playerRoot.position - _playerBody.position;
        Vector3 targetRootPosition = _targetBodyPosition + rootToBodyOffset;
        _playerRoot.position = targetRootPosition;

        Rigidbody rootRigidbody = _playerRoot.GetComponent<Rigidbody>();

        if (rootRigidbody != null)
        {
            rootRigidbody.linearVelocity = Vector3.zero;
            rootRigidbody.angularVelocity = Vector3.zero;
        }

        Rigidbody bodyRigidbody = _playerBody.GetComponent<Rigidbody>();

        if (bodyRigidbody == null)
        {
            return;
        }

        if (bodyRigidbody == rootRigidbody)
        {
            return;
        }

        bodyRigidbody.linearVelocity = Vector3.zero;
        bodyRigidbody.angularVelocity = Vector3.zero;
    }

    private void OnGenerationCompleted()
    {
        Vector3 startRoomCenterPosition = _levelGenerator.GetStartRoomCenter();
        startRoomCenterPosition.y = startRoomCenterPosition.y + _bodyHeight;

        _targetBodyPosition = startRoomCenterPosition;
        _syncFramesLeft = Mathf.Max(_syncFrames, 1);

        CenterPlayerBody();
    }
}
