using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [Header("Зависимости")]
    [SerializeField] private CharacterMover _movement;
    [SerializeField] private CameraMover _cameraMover;
    [SerializeField] private CursorManager _cursor;
    [SerializeField] private Camera _mainCamera;

    private PlayerInputActions _inputs;

    private void Awake()
    {
        _inputs = new PlayerInputActions();

        _inputs.Player.Move.performed += OnMovePerformed;
        _inputs.Player.Move.canceled += OnMoveCanceled;

        _inputs.Player.Jump.performed += OnJumpPerformed;

        _inputs.Player.Sprint.performed += OnSprintPerformed;
        _inputs.Player.Sprint.canceled += OnSprintCanceled;

        // _inputs.Player.Look.performed += OnLookPerformed; 
    }

    private void OnEnable() => _inputs.Enable();
    private void OnDisable() => _inputs.Disable();

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _movement?.OnMove(input);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _movement?.OnMove(Vector2.zero);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _movement?.OnJump();
    }

    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        _movement?.OnSprint(true);
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        _movement?.OnSprint(false);
    }
}
