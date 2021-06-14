using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileStone : ProjectileBase
{
    protected override void Start()
    {
        type = "Stone";
        thrownByPlayer = 100;
        explosionRadius = 5f;
        explosionDamage = 15f;
        base.Start();
    }


    public override void Initialize(Vector3 _initialMovementDirection, float _initialForceStrenght, int _thrownByPlayer)
    {
        initialForce = _initialMovementDirection * _initialForceStrenght;
    }
}
