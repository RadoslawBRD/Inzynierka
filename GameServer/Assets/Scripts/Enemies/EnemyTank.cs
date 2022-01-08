using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : Enemy
{

    public float throwRange;
    public float throwForce ;
    public string projectileType;
    protected override void Start()
    {

        gravity = -9.81f;
        patrolSpeed = 1f;
        chaseSpeed = 3.5f;
        health = 250f;
        maxHealth = 250f;
        detectionRange = 30f;
        shootRange = 2f;
        shootAccuracy = 0.00001f;
        patrolDuration = 3f;
        idleDuration = 1f;
        damage = 60f;
        type = "Tank";
        throwRange = 15f;
        throwForce = 250f;
        projectileType = "Stone";
        base.Start();
    }

    protected override void Attack()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;
            transform.forward = new Vector3(_enemyToPlayer.x, 0f, _enemyToPlayer.z);

            if (_enemyToPlayer.magnitude <= shootRange)
            {
                Shoot(_enemyToPlayer);
            }else if(_enemyToPlayer.magnitude >= shootRange && _enemyToPlayer.magnitude <= throwRange)
                {
                Debug.Log("próbuje rzuciæ no");
                ThrowItem(transform.forward*3+transform.up,"Stone");
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
    public void ThrowItem(Vector3 _viewDirection, string _type)
    {
        if (Random.Range(0,100)<5)
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
