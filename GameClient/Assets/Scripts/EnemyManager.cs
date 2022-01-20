using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int id;
    public float health;
    public float maxHealth;
    public EnemyState state = EnemyState.idle;


    private Animator animator;

    public void Initialize(int _id, float _maxHealth)
    {
        id = _id;
        health = _maxHealth;
        maxHealth = _maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void SetHealth(float _healh)
    {
        health = _healh;
        if (health <= 0)
        {
            GameManager.enemies.Remove(id);
            Destroy(gameObject);
        }
    }
    public void SetState(EnemyState _state)
    {
        state = _state;
        switch (_state)
        {
            case EnemyState.idle:
                animator.SetInteger("ChangeState", 1);
                break;
            case EnemyState.patrol:
                
                animator.SetInteger("ChangeState", 1);
                break;
            case EnemyState.chase:
                animator.SetInteger("ChangeState", 3);
                break;
            case EnemyState.attack:
                animator.SetInteger("ChangeState", 4);
                break;
            default:
                animator.SetInteger("ChangeState", 1);

                break;
        }        
    }
}
public enum EnemyState
{
    idle,
    patrol,
    chase,
    attack
}
