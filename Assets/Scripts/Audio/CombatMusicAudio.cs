using System;
using System.Collections.Generic;
using System.Collections;
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
    private Coroutine _fadeCoroutine;

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
        StopFadeRoutine();

        if (_audioSource != null)
        {
            _audioSource.volume = 0f;
            _audioSource.Stop();
        }
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
            EnsurePlaybackStarted();
            EnsureFadeRoutine();
            return;
        }

        if (_isCombatActive)
        {
            _isCombatActive = false;
            _releaseTimer = _releaseDelay;
            EnsureFadeRoutine();
            return;
        }

        if (_audioSource.volume > 0f)
        {
            EnsureFadeRoutine();
            return;
        }

        StopPlaybackIfSilent();
    }

    private IEnumerator FadeRoutine()
    {
        while (true)
        {
            if (_isCombatActive == false && _releaseTimer > 0f)
            {
                _releaseTimer = Mathf.Max(0f, _releaseTimer - Time.deltaTime);
            }

            float targetVolume = 0f;

            if (_isCombatActive || _releaseTimer > 0f)
            {
                targetVolume = _targetVolume;
            }

            if (targetVolume > 0f)
            {
                EnsurePlaybackStarted();
            }

            _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, targetVolume, _fadeSpeed * Time.deltaTime);

            if (Mathf.Abs(_audioSource.volume - targetVolume) <= 0.0001f)
            {
                _audioSource.volume = targetVolume;

                if (targetVolume <= 0f)
                {
                    StopPlaybackIfSilent();
                }

                if (_isCombatActive == false && _releaseTimer <= 0f)
                {
                    _fadeCoroutine = null;
                    yield break;
                }
            }

            yield return null;
        }
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

    private void EnsurePlaybackStarted()
    {
        if (_audioSource.isPlaying)
        {
            return;
        }

        _audioSource.Play();
    }

    private void StopPlaybackIfSilent()
    {
        if (_audioSource.volume > 0f)
        {
            return;
        }

        if (_audioSource.isPlaying == false)
        {
            return;
        }

        _audioSource.Stop();
    }

    private void EnsureFadeRoutine()
    {
        if (_fadeCoroutine != null)
        {
            return;
        }

        _fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private void StopFadeRoutine()
    {
        if (_fadeCoroutine == null)
        {
            return;
        }

        StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = null;
    }
}
