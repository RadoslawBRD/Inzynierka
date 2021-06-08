using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBasic : Enemy
{

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
        base.Start();
    }
   


}
