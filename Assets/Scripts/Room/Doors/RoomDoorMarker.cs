using UnityEngine;

public sealed class RoomDoorMarker : MonoBehaviour
{
    [SerializeField] private DoorSide _side;
    [SerializeField] private DoorRole _role;
    [SerializeField, Min(1)] private int _widthInBlocks = 2;
    [SerializeField, Min(1)] private int _heightInBlocks = 2;

    public DoorSide Side => _side;
    public DoorRole Role => _role;
    public int WidthInBlocks => _widthInBlocks;
    public int HeightInBlocks => _heightInBlocks;

    public void Initialize(DoorSide side, DoorRole role, int widthInBlocks, int heightInBlocks)
    {
        _side = side;
        _role = role;
        _widthInBlocks = Mathf.Max(1, widthInBlocks);
        _heightInBlocks = Mathf.Max(1, heightInBlocks);
    }
}
