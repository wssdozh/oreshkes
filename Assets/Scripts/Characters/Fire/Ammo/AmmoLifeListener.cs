using System;
using UnityEngine;

public abstract class AmmoLifeListener : MonoBehaviour
{
    [SerializeField] private Ammo _ammo;

    protected Ammo Ammo => _ammo;

    protected virtual void Awake()
    {
        if (_ammo == null)
        {
            throw new InvalidOperationException(nameof(_ammo));
        }
    }

    protected virtual void OnEnable()
    {
        _ammo.LifeEnded += HandleAmmoLifeEnded;

        OnAmmoEnabled();
    }

    protected virtual void OnDisable()
    {
        _ammo.LifeEnded -= HandleAmmoLifeEnded;
    }

    private void HandleAmmoLifeEnded()
    {
        OnAmmoLifeEnded();
    }

    protected virtual void OnAmmoEnabled()
    {
    }

    protected virtual void OnAmmoLifeEnded()
    {
    }
}
