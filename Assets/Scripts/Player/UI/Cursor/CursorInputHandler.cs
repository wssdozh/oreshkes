using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class CursorInputHandler : MonoBehaviour
{
    private readonly List<RaycastResult> _uiRaycastResults = new List<RaycastResult>(8);

    [SerializeField] private CursorManager _cursorManager;
    [SerializeField] private CursorAnimator _cursorAnimator;
    [SerializeField] private float _holdThresholdSeconds = 0.18f;

    private PlayerInputActions _inputs;
    private Coroutine _holdCoroutine;
    private WaitForSecondsRealtime _holdWait;
    private EventSystem _uiEventSystem;
    private PointerEventData _uiPointerEventData;
    private Player _player;

    private bool _isAttackPressed;
    private bool _isHoldConfirmed;

    private void Awake()
    {
        _inputs = new PlayerInputActions();
        PlayerInputBindingOverrideStore.Apply(_inputs);
        _holdWait = new WaitForSecondsRealtime(_holdThresholdSeconds);
        ResolveCursorManager();
        ResolvePlayer();
    }

    private void OnEnable()
    {
        PlayerInputBindingOverrideStore.Changed += OnBindingOverridesChanged;
        PlayerInputBindingOverrideStore.Apply(_inputs);
        _inputs.Enable();

        _inputs.Player.Attack.performed += OnAttackPerformed;
        _inputs.Player.Attack.canceled += OnAttackCanceled;

        _inputs.Player.UseItem.performed += OnUseItemPerformed;
        _inputs.Player.Scroll.performed += OnScrollPerformed;
    }

    private void OnDisable()
    {
        PlayerInputBindingOverrideStore.Changed -= OnBindingOverridesChanged;

        _inputs.Player.Attack.performed -= OnAttackPerformed;
        _inputs.Player.Attack.canceled -= OnAttackCanceled;

        _inputs.Player.UseItem.performed -= OnUseItemPerformed;
        _inputs.Player.Scroll.performed -= OnScrollPerformed;

        _inputs.Disable();

        _isAttackPressed = false;
        _isHoldConfirmed = false;
        StopHoldLoop();

        _cursorAnimator.ResetToBase();
    }

    private void OnDestroy()
    {
        if (_inputs == null)
        {
            return;
        }

        _inputs.Dispose();
        _inputs = null;
    }

    private void LateUpdate()
    {
        ResolveCursorManager();
        ResolvePlayer();

        if (_cursorManager == null)
        {
            _cursorAnimator.SetHoverVisual(IsPlayerInBattle());

            return;
        }

        _cursorAnimator.SetHoverVisual(IsPlayerInBattle() || _cursorManager.HasDamageableHit);
    }

    private void OnAttackPerformed(InputAction.CallbackContext callbackContext)
    {
        if (IsPointerOverUi())
        {
            return;
        }

        _isAttackPressed = true;
        _isHoldConfirmed = false;
        StartHoldLoop();

        _cursorAnimator.BeginHoldCandidate(_holdThresholdSeconds);
    }

    private void OnAttackCanceled(InputAction.CallbackContext callbackContext)
    {
        if (_isAttackPressed == false)
        {
            return;
        }

        if (_isHoldConfirmed)
        {
            _cursorAnimator.EndHold();
        }
        else
        {
            _cursorAnimator.CancelHoldCandidate();
            _cursorAnimator.PlayClick();
        }

        _isAttackPressed = false;
        _isHoldConfirmed = false;
        StopHoldLoop();
    }

    private void OnUseItemPerformed(InputAction.CallbackContext callbackContext)
    {
        if (IsPointerOverUi())
        {
            return;
        }

        _cursorAnimator.PlaySecondaryClick();
    }

    private void OnScrollPerformed(InputAction.CallbackContext callbackContext)
    {
        if (IsPointerOverUi())
        {
            return;
        }

        Vector2 scrollValue = callbackContext.ReadValue<Vector2>();
        float direction = scrollValue.y;

        if (Mathf.Abs(direction) <= Mathf.Epsilon)
        {
            direction = scrollValue.x;
        }

        _cursorAnimator.PlayScroll(direction);
    }

    private IEnumerator HoldLoop()
    {
        yield return _holdWait;

        if (_isAttackPressed == false)
        {
            _holdCoroutine = null;

            yield break;
        }

        if (_isHoldConfirmed == false)
        {
            _isHoldConfirmed = true;
            _cursorAnimator.ConfirmHold();
        }

        _holdCoroutine = null;
    }

    private void StartHoldLoop()
    {
        StopHoldLoop();
        _holdWait = new WaitForSecondsRealtime(_holdThresholdSeconds);
        _holdCoroutine = StartCoroutine(HoldLoop());
    }

    private void StopHoldLoop()
    {
        if (_holdCoroutine == null)
        {
            return;
        }

        StopCoroutine(_holdCoroutine);
        _holdCoroutine = null;
    }

    private void OnBindingOverridesChanged()
    {
        _inputs.Disable();
        PlayerInputBindingOverrideStore.Apply(_inputs);
        _inputs.Enable();
    }

    private void ResolveCursorManager()
    {
        if (_cursorManager != null)
        {
            return;
        }

        _cursorManager = CursorManager.Instance;
    }

    private void ResolvePlayer()
    {
        if (_player != null)
        {
            return;
        }

        _player = Player.Instance;
    }

    private bool IsPlayerInBattle()
    {
        if (_player == null)
        {
            return false;
        }

        return _player.IsInBattle;
    }

    private bool IsPointerOverUi()
    {
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            return false;
        }

        Pointer pointer = Pointer.current;

        if (pointer == null)
        {
            return false;
        }

        if (_uiPointerEventData == null || _uiEventSystem != eventSystem)
        {
            _uiEventSystem = eventSystem;
            _uiPointerEventData = new PointerEventData(eventSystem);
        }

        _uiPointerEventData.Reset();
        _uiPointerEventData.position = pointer.position.ReadValue();
        _uiRaycastResults.Clear();
        eventSystem.RaycastAll(_uiPointerEventData, _uiRaycastResults);

        return _uiRaycastResults.Count > 0;
    }
}
