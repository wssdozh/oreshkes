using System;
using UnityEngine;

public class BulletFireExecutor : FireExecutor
{
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Ammo _bulletPrefab;

    private AmmoSpawner _bulletSpawner;

    private void Start()
    {
        _bulletSpawner = SpawnerServiceLocator.Get<Ammo>(_bulletPrefab.name) as AmmoSpawner;

        if (_bulletSpawner == null)
        {
            throw new InvalidOperationException(nameof(_bulletSpawner));
        }
    }

    protected override bool TryFireInternal()
    {
        RotateMuzzleToAimPoint(_muzzle);

        _bulletSpawner.Spawn(_muzzle.position, _muzzle.rotation, TargetLayers);

        return true;
    }
}
