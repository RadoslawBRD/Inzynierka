using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public static int maxEnemies = 10;
    public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();
    private static int nextEnemyId = 0;

    public int id;
    public EnemyState state;
    public Player target;
    public CharacterController controller;
    public Transform shootOrigin;
    protected NavMeshAgent navMeshaAgent;
    private Animator animator;

    public float gravity = -9.81f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 6f;
    public float health=100f;
    public float maxHealth = 100f;
    public float detectionRange = 30f;
    public float shootRange = 2f;
    public float shootAccuracy = 0.1f;
    public float patrolDuration = 3f;
    public float idleDuration = 1f;
    public float damage = 40f;
    public string type = "Basic";


    public string typzombie;

    private Vector2 _randomPatrolDirection= new Vector2(0,0);
    private bool isPatrolRoutineRunning;
    private float yVelocity = 0;
    protected virtual void Start()
    {
        id = nextEnemyId;
        nextEnemyId++;
        enemies.Add(id, this);
        
        ServerSend.SpawnEnemy(this);

        state = EnemyState.patrol;
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        patrolSpeed *= Time.fixedDeltaTime;
        chaseSpeed *= Time.fixedDeltaTime;
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(initializeNavMeshAgent());
    }

    private IEnumerator initializeNavMeshAgent()
    {
        yield return new WaitForSeconds(1);
        navMeshaAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshaAgent.enabled = true;

        //navMeshaAgent = gameObject.GetComponent<NavMeshAgent>();

    }

    protected void FixedUpdate()
    {
        
        switch (state)
        {
            case EnemyState.idle:
                LookForPlayer();
                animator.SetInteger("ChangeState", 1);
                break;
            case EnemyState.patrol:
                if (!LookForPlayer())
                    Patrol();
                animator.SetInteger("ChangeState", 1);
                break;
            case EnemyState.chase:
                Chase();
                animator.SetInteger("ChangeState", 3);
                break;
            case EnemyState.attack:
                Attack();
                animator.SetInteger("ChangeState", 4);
                break;
            default:
                animator.SetInteger("ChangeState", 1);

                break;
        }
        //this.transform.rotation = Quaternion.LookRotation(_randomPatrolDirection, transform.position);
    }

    private bool LookForPlayer()
    {
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                Vector3 _enemyToPlayer = _client.player.transform.position - transform.position;
                if (_enemyToPlayer.magnitude <= detectionRange)
                {
                    if (Physics.Raycast(shootOrigin.position, _enemyToPlayer, out RaycastHit _hit, detectionRange))
                    {
                        if (_hit.collider.CompareTag("Player"))
                        {
                            target = _hit.collider.GetComponent<Player>();
                            if (isPatrolRoutineRunning)
                            {
                                isPatrolRoutineRunning = false;
                                StopCoroutine(StartPatrol());
                            }

                            state = EnemyState.chase;
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private void Patrol()
    {
        if (!isPatrolRoutineRunning)
        {
            StartCoroutine(StartPatrol());
        }
        NewMove("Patrol",null,patrolSpeed);
        //Move(transform.forward, patrolSpeed);
    }

    private IEnumerator StartPatrol()
    {
        isPatrolRoutineRunning = true;
        _randomPatrolDirection = Random.insideUnitCircle.normalized;
        _randomPatrolDirection = Random.insideUnitCircle.normalized;
        //navMeshaAgent.nextPosition = _randomPatrolDirection;
       // transform.forward = new Vector3(_randomPatrolDirection.x, 0f, _randomPatrolDirection.y);

        yield return new WaitForSeconds(patrolDuration);

        state = EnemyState.idle;

        yield return new WaitForSeconds(idleDuration);

        state = EnemyState.patrol;
        isPatrolRoutineRunning = false;
    }

    protected virtual void Chase()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;

            if (_enemyToPlayer.magnitude <= shootRange)
            {
                state = EnemyState.attack;
            }
            else
            {
               NewMove("Chase", target.transform.position, chaseSpeed);
            }
        }
        else
        {
            target = null;
            if(navMeshaAgent.remainingDistance < .5f)              
                state = EnemyState.patrol;
        }
    }

    protected virtual void Attack()
    {
        if (CanSeeTarget())
        {
            Vector3 _enemyToPlayer = target.transform.position - transform.position;
            transform.forward = new Vector3(_enemyToPlayer.x, 0f, _enemyToPlayer.z);

            if (_enemyToPlayer.magnitude <= shootRange)
            {
                Shoot(_enemyToPlayer);
            }
            else
            {
                NewMove("Chase", target.transform.position, chaseSpeed);

                //MoveNav();
                //Move(_enemyToPlayer, chaseSpeed);
            }
        }
        else
        {
            target = null;
            state = EnemyState.patrol;
        }
    }
    protected void MoveNav()
    {
        if (navMeshaAgent != null) //navMeshaAgent.gameObject.GetComponent<NavMeshAgent>().enabled ||
        {
            navMeshaAgent.destination = target.transform.position;
            this.transform.rotation = Quaternion.LookRotation(navMeshaAgent.nextPosition - this.transform.position);
            //this.transform.rotation = Quaternion.LookRotation(navMeshaAgent.nextPosition);//- this.transform.position);
            ServerSend.EnemyPosition(this);
            //TODO: zoombie ma randomowy obrót
        }

    }
    private Vector3 PatrolRandomTarget()
    {
        NavMeshHit navHit;
        Vector3 randomDirection = Random.insideUnitSphere * 2f;
        NavMesh.SamplePosition(randomDirection+=this.transform.position, out navHit, 2f, -1);
        Debug.Log("Szukam");
        return navHit.position;
        
    }
    protected void NewMove(string _mode, Vector3? _target, float _speed)
    {
        try
        {
            if (navMeshaAgent != null)
                switch (_mode)
                {
                    case "Patrol":
                        try
                        {
                            if (navMeshaAgent.remainingDistance < .1f)
                            {
                                float baseAnimationSpeed = animator.speed;
                                animator.speed = .6f;
                                navMeshaAgent.speed = 1f;
                                Vector3 destinationWalk = PatrolRandomTarget();
                                Vector3 walkDirection = navMeshaAgent.destination - transform.position;
                                this.transform.rotation = Quaternion.LookRotation(walkDirection);
                                navMeshaAgent.destination = destinationWalk;
                                Debug.DrawRay(transform.position, walkDirection, Color.red);
                                animator.speed = baseAnimationSpeed;
                            }
                        }
                        catch { }
                        break;
                    case "Chase":
                        try
                        {
                            if (navMeshaAgent.remainingDistance < .2f)
                            {
                                navMeshaAgent.speed = 5.2f;

                                Vector3 walkDirection = (Vector3)(_target - transform.position);
                                this.transform.rotation = Quaternion.LookRotation(walkDirection);
                                navMeshaAgent.destination = (Vector3)_target;
                                Debug.DrawRay(transform.position, walkDirection, Color.red);
                                DrawPath();
                                ServerSend.EnemyPosition(this);
                            }
                        }
                        catch { }
                        break;
                    case "Idle":
                        break;

                    default:
                        break;

                }
        }
        catch { }
        ServerSend.EnemyPosition(this);
    }
    private void DrawPath()
    {
        NavMeshPath path = navMeshaAgent.path;
        Vector3 prevX = transform.position;
        foreach (Vector3 x in path.corners){

            Debug.DrawRay(prevX, x, Color.red, 1f);
            prevX = x;

        }
    }
    protected void Move(Vector3 _direction, float _speed)
    {
        _direction.y = 0f;
        transform.forward = _direction;
        Vector3 _movement = transform.forward * _speed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
        }
        yVelocity += gravity;

        _movement.y = yVelocity;
        controller.Move(_movement);

        ServerSend.EnemyPosition(this);
    }

    protected void Shoot(Vector3 _shootDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _shootDirection, out RaycastHit _hit, shootRange))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                Debug.Log($"Hit in{_hit.collider.gameObject.ToString()}");
                if (Random.value <= shootAccuracy)
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(damage);
                }
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        Debug.Log($"dostaje damage{type}");
        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            GameManager.instance.UpdateKillScore();
            enemies.Remove(id);
            Destroy(gameObject);
        }

        ServerSend.EnemyHealth(this);
    }

    protected bool CanSeeTarget()
    {
        if (target == null)
        {
            return false;
        }

        if (Physics.Raycast(shootOrigin.position, target.transform.position - transform.position, out RaycastHit _hit, detectionRange))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}

public enum EnemyState
{
    idle, //1
    patrol, //1
    chase, //3
    attack //4
}