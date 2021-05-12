using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public bool isMaster = false;

    public CharacterController controller;
    public float gravity = -9.81f; //sila grawitacji
    public Transform shootOrigin;
    
    public float moveSpeed = 5.0f;
    public float jumpSpeed = 5.0f;

    public float throwForce = 600f;
    public float health;
    public float maxHealth=100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;


    private bool[] inputs;
    private float yVelocity = 0; //pr�dko�c poruszania si� wertykalnie

    private void Start()
    {
        
        gravity *= Time.deltaTime * Time.deltaTime;
        //moveSpeed *= Time.deltaTime;
        jumpSpeed *= Time.deltaTime;
    }
    public void SetMaster()
    {
        isMaster = true;
    }
    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        
        inputs = new bool[5];
    }
    public void FixedUpdate()
    {
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
            yVelocity = 0f; //aby dalej nie opada�
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        if (!isMaster)
        {
            yVelocity += gravity; // je�li nie stoi na ziemi to spadaj
            _moveDirection.y = yVelocity; //kierunek w kt�rym posta� 'spada'
        }

        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
    public void Shoot(Vector3 _viewDirection)
    {
        if (isMaster) //strezlanie mastera(respienie zombie)
        {
            if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 1125f))
            {
                if (_hit.collider.CompareTag("Ground"))
                {
                    NetworkManager.instance.InstantiateEnemy(_hit.point);
                }
            }

        }
        else
        { // strzelanie zwyklego gracza
            if (health <= 0f)
            {
                return;
            }
            if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
            {
                if (_hit.collider.CompareTag("Player"))
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(50f);
                }
                else
                    if (_hit.collider.CompareTag("Enemy"))
                {
                    _hit.collider.GetComponent<Enemy>().TakeDamage(50f);
                }
            }
        }
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0)
        {
            return;
        }
        if(itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    public void TakeDamage(float _damage)
    {
        if(health <= 0f)
        {
            return;
        }
        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }
        ServerSend.PlayerHealth(this);
    }
    private IEnumerator Respawn()
    {
        if (isMaster)
        {
            gameObject.GetComponent<Player>().tag = "Master";
            gravity = 0;
            yVelocity = 0;
        }
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
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
}
