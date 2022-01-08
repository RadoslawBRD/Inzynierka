using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpiter : Enemy
{
    public float throwRange;
    public float throwForce;
    public string projectileType;

    protected override void Start()
    {
        gravity = -9.81f;
        patrolSpeed = 2f;
        chaseSpeed = 8f;
        health = 200f;
        maxHealth = 200f;
        detectionRange = 30f;
        shootRange = 2f;
        shootAccuracy = 0.1f;
        patrolDuration = 3f;
        idleDuration = 1f;
        damage = 10f;
        type = "Basic";
        base.Start();
    }

    public void ThrowItem(Vector3 _viewDirection, string _type)
    {
        if (Random.Range(0, 100) < 5)
        //if (true)
        {
            Debug.Log("rzucam no");
            NetworkManager.instance.InstantiateProjectile(shootOrigin, projectileType).Initialize(_viewDirection, throwForce, id);
        }
    }
    protected override void Chase()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;

            if (_enemyToPlayer.magnitude <= shootRange || _enemyToPlayer.magnitude <= throwRange)
            {
                state = EnemyState.attack;
            }
            else
            {
                Move(_enemyToPlayer, chaseSpeed);
            }
        }
        else
        {
            target = null;
            state = EnemyState.patrol;
        }
    }
}
