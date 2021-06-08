using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int id;
    public float health;
    public float maxHealth;

    public void Initialize(int _id, float _maxHealth)
    {
        id = _id;
        health = _maxHealth;
        maxHealth = _maxHealth;
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
}
