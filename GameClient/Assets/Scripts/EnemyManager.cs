using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int id;
    public float health;
    public float maxHealth;
    public EnemyState state = EnemyState.idle;

    private Vector3 position;
    private Vector3 oldPosition;
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
            try
            {
                Destroy(gameObject.GetComponent<GameObject>());
                Destroy(gameObject);
            }
            catch
            {
                Debug.Log("Problem przy zabijaniu zombie");
            }
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
        position = this.transform.position;
        if (Vector3.Distance(position, oldPosition) > 5)
        {
            Debug.LogWarning("skok pozycji, prawdopodobny lag");
        }
        oldPosition = position;
    }
}
public enum EnemyState
{
    idle,
    patrol,
    chase,
    attack
}
