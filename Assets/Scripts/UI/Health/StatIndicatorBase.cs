using System;
using UnityEngine;

public abstract class StatIndicatorBase<T> : MonoBehaviour where T : Stat
{
    [SerializeField] protected T Stat;

    public void SetStat(T stat)
    {
        if (stat == null)
        {
            throw new InvalidOperationException(nameof(stat));
        }

        if (ReferenceEquals(Stat, stat))
        {
            Display();

            return;
        }

        if (isActiveAndEnabled && Stat != null)
        {
            Stat.Changed -= Display;
        }

        Stat = stat;

        if (isActiveAndEnabled)
        {
            Stat.Changed += Display;
        }

        Display();
    }

    public void ClearStat()
    {
        if (isActiveAndEnabled && Stat != null)
        {
            Stat.Changed -= Display;
        }

        Stat = null;
    }

    protected virtual void OnEnable()
    {
        if (Stat == null)
        {
            return;
        }

        Stat.Changed += Display;
    }

    protected virtual void OnDisable()
    {
        if (Stat == null)
        {
            return;
        }

        Stat.Changed -= Display;
    }

    protected virtual void Start()
    {
        if (Stat == null)
        {
            return;
        }

        Display();
    }

    protected abstract void Display();
}
