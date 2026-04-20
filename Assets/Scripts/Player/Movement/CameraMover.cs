using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [Header("Р—Р°РІРёСЃРёРјРѕСЃС‚Рё")]
    [SerializeField] private Transform _player;
    [SerializeField] private CursorManager _cursorManager;
    [SerializeField] private Camera _camera;

    [Header("РќР°СЃС‚СЂРѕР№РєРё")]
    [SerializeField] private float _height = 10f;
    [SerializeField] private float _smoothSpeed = 5f;
    [Range(0f, 1f)][SerializeField] private float _mouseInfluence = 0.3f;

    [Header("РЎРјРµС‰РµРЅРёРµ & РџРѕРІРѕСЂРѕС‚")]
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;
    [SerializeField, Range(30f, 90f)] private float _tiltAngle = 60f;

    private Vector3 _targetPosition;

    private void LateUpdate()
    {
        Vector3 playerPos = _player.position;
        Vector3 mixedPoint = Vector3.Lerp(playerPos, _cursorManager.MouseGroundPos, _mouseInfluence);

        _targetPosition = new Vector3(mixedPoint.x, _player.transform.position.y + _height, mixedPoint.z) + _positionOffset;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, _smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(_tiltAngle, 0f, 0f);
    }
}
