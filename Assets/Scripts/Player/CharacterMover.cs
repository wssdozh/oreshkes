using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]
public class CharacterMover : MonoBehaviour
{
    [Header("Зависимости")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ForceMode _forceMode;

    [Header("Настройки")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _speedSprint = 6f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _rayLengthGround = 0.2f;
    [SerializeField] private LayerMask _groundLayer = 1;

    private Vector3 _moveDirection;
    private bool _isSprinting;

    private bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, _rayLengthGround, _groundLayer);

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(Vector2 input)
    {
        _moveDirection = new Vector3(input.x, 0f, input.y);
    }

    public void OnSprint(bool sprinting)
    {
        _isSprinting = sprinting;
    }

    public void OnJump()
    {
        if (IsGrounded)
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void Move()
    {        
        if (_moveDirection.sqrMagnitude < 0.01f)
        {
            // _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f); 
            return;
        }

        float speed = _isSprinting ? _speedSprint : _speed;
        Vector3 targetVelocity = _moveDirection.normalized * speed;

        _rigidbody.linearVelocity = new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _rayLengthGround);
    }
}
