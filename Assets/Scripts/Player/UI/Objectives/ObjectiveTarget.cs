using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ObjectiveTarget : MonoBehaviour
{
    [SerializeField] private string _id;
    [SerializeField] private Health _health;

    private bool _isCompleted;

    public static event Action<string> Completed;

    public string Id => _id;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        Completed = null;
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.Ended += OnHealthEnded;
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.Ended -= OnHealthEnded;
        }

        _isCompleted = false;
    }

    public bool HasId(string id)
    {
        return string.Equals(_id, id, StringComparison.Ordinal);
    }

    public void Complete()
    {
        if (string.IsNullOrWhiteSpace(_id))
        {
            throw new InvalidOperationException(nameof(_id));
        }

        if (_isCompleted == true)
        {
            return;
        }

        _isCompleted = true;

        Action<string> completed = Completed;

        if (completed != null)
        {
            completed.Invoke(_id);
        }
    }

    private void OnHealthEnded()
    {
        Complete();
    }
}
