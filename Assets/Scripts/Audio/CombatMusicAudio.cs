using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CombatMusicAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField, Min(0f)] private float _targetVolume = 0.14f;
    [SerializeField, Min(0f)] private float _fadeSpeed = 0.6f;
    [SerializeField, Min(0f)] private float _releaseDelay = 1.2f;

    private bool _isCombatActive;
    private float _releaseTimer;

    private void Awake()
    {
        if (_audioSource == null)
            throw new InvalidOperationException(nameof(_audioSource));

        if (_targetVolume < 0f)
            throw new InvalidOperationException(nameof(_targetVolume));

        if (_fadeSpeed < 0f)
            throw new InvalidOperationException(nameof(_fadeSpeed));

        if (_releaseDelay < 0f)
            throw new InvalidOperationException(nameof(_releaseDelay));

        _audioSource.playOnAwake = true;
        _audioSource.loop = true;
        _audioSource.volume = 0f;

        if (_audioSource.isPlaying == false)
            _audioSource.Play();
    }

    private void OnEnable()
    {
        RoomCombatLock.StateChanged += OnRoomCombatLockStateChanged;
        RefreshCombatState();
    }

    private void OnDisable()
    {
        RoomCombatLock.StateChanged -= OnRoomCombatLockStateChanged;
        _isCombatActive = false;
        _releaseTimer = 0f;

        if (_audioSource != null)
        {
            _audioSource.volume = 0f;
            _audioSource.Stop();
        }
    }

    private void Update()
    {
        TickReleaseTimer();

        float targetVolume = _isCombatActive || _releaseTimer > 0f ? _targetVolume : 0f;
        _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, targetVolume, _fadeSpeed * Time.deltaTime);
    }

    private void OnRoomCombatLockStateChanged()
    {
        RefreshCombatState();
    }

    private void RefreshCombatState()
    {
        bool hasActiveCombat = HasActiveCombat();

        if (hasActiveCombat)
        {
            _isCombatActive = true;
            _releaseTimer = 0f;
            return;
        }

        if (_isCombatActive)
        {
            _isCombatActive = false;
            _releaseTimer = _releaseDelay;
        }
    }

    private void TickReleaseTimer()
    {
        if (_isCombatActive)
            return;

        if (_releaseTimer <= 0f)
            return;

        _releaseTimer = Mathf.Max(0f, _releaseTimer - Time.deltaTime);
    }

    private bool HasActiveCombat()
    {
        IReadOnlyList<RoomCombatLock> roomCombatLocks = RoomCombatLock.Instances;

        for (int i = 0; i < roomCombatLocks.Count; i++)
        {
            RoomCombatLock roomCombatLock = roomCombatLocks[i];

            if (roomCombatLock != null && roomCombatLock.IsLocked)
                return true;
        }

        return false;
    }
}
