using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflinePlayerManager : MonoBehaviour
{
    public int id;
    public string username="Offline player";
    
    public MeshRenderer model;

    public CharacterController controller;
    public float gravity = -9.81f; //sila grawitacji
    public Transform shootOrigin;

    public float moveSpeed = 5.0f;
    public float jumpSpeed = 5.0f;

    public float throwForce = 600f;
    public float health=100;
    public float maxHealth=100;
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    public float yVelocity;
    bool[] inputs;
    public void Initialize()
    {
        gravity *= Time.deltaTime * Time.deltaTime;
        //moveSpeed *= Time.deltaTime;
        jumpSpeed *= Time.deltaTime;
    }

    public void FixedUpdate()
    {
        inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        if (health <= 0f)
        {
            return;
        }

        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection)
    {

        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f; //aby dalej nie opada³
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        
        controller.Move(_moveDirection);

        
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
    public void Shoot(Vector3 _viewDirection)
    {
        
         // strzelanie zwyklego gracza
            if (health <= 0f)
            {
                return;
            }
            if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
            {
                
            }
        
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        Vector3 test = shootOrigin.position + shootOrigin.forward * 0.7f;
        if (health <= 0)
        {
            return;
        }
        if (itemAmount > 0)
        {
            itemAmount--;
            //Projectile.Initialize(test, throwForce);
        }
    }



    
    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }
        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            Respawn();
        }
        SetHealth(health);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;

    }


    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        model.enabled = false;
    }
    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }
}
