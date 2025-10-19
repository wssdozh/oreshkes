using UnityEngine;


class CharacterRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private CursorManager _cursorManager;

    private void FixedUpdate()
    {
        Rotate();       
    }

    private void Rotate()
    {
        Vector3 direction = (_cursorManager.MouseWorldPos - transform.position).normalized;
        direction.y = 0;

        float rotationFactor = 100f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                _rotationSpeed * Time.fixedDeltaTime * rotationFactor
            );
        }
    }
}