using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<int, string> selectedEnemy = new Dictionary<int, string>();

        
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
    public int moneyCount = 0;
    public float maxHealth=100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;

    private int ammoCost = 5;

    public string _type = "Basic";
    public int selectedEnemyInt = 1;
    private bool[] inputs;
    private float yVelocity = 0; //prêdkoœc poruszania siê wertykalnie

    private void Start()
    {
        selectedEnemy[1] = "Basic";
        selectedEnemy[2] = "Tank";

        gravity = -9.81f;
        gravity *= Time.deltaTime * Time.deltaTime;
        //moveSpeed *= Time.deltaTime;
        jumpSpeed = 5.0f;
        jumpSpeed *= Time.deltaTime;
    }
    public void SetMaster(bool value)
    {
        isMaster = value;
        if (value)
        {
            gameObject.tag = "Master";
        }
        else
        {
            gameObject.tag = "Player";
        }
    }
    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        
        inputs = new bool[6];
    }
    public void SetSelectedEnemy(string _type)
    {
        switch (_type)
        {
            case "Basic":
                selectedEnemyInt = 1;
                break;
            case "Tank":
                selectedEnemyInt = 2;
                break;
            default:
                selectedEnemyInt = 1;
                break;
        }

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
            yVelocity = 0f; //aby dalej nie opada³
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        if (!isMaster)
        {
            yVelocity += gravity; // jeœli nie stoi na ziemi to spadaj
            _moveDirection.y = yVelocity; //kierunek w którym postaæ 'spada'
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
    public void Shoot(Vector3 _viewDirection, string _type)
    {
        if (isMaster) //strzelanie mastera(respienie zombie)
        {
            if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hitGround, 1125f))
            {
                if (_hitGround.collider.CompareTag("Ground"))
                {
                    bool isPlayerAway = true;
                    Vector3 _masterHit = _hitGround.point; // powinno byæ git

                    foreach(Client _player in Server.clients.Values) 
                    {
                        if(_player.player!=null)
                            if (_player.player.isMaster == true)
                            {

                            }else
                                if (Vector3.Distance(_player.player.transform.position, _masterHit) < 25) //TODO: dopracowaæ odleg³oœæ
                                    isPlayerAway = false;
                    }
                        
                    if (isPlayerAway)
                    {
                        Debug.Log(_type);
                        NetworkManager.instance.InstantiateEnemy(_hitGround.point, _type);
                    }
                    else
                    {
                        Debug.Log("IsPlayerAway");
                    }
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
                Debug.Log($"Hit in{_hit.collider.gameObject}");

                if (_hit.collider.gameObject.CompareTag("Enemy"))
                {
                    if (_hit.collider.gameObject.GetComponentInParent<Enemy>().health <= 50f) /// wzrost œrodków za zabicie
                    {
                        if (_hit.collider.gameObject.GetComponentInParent<Enemy>().type == "Basic")

                            moneyCount += 10;
                        else
                            if (_hit.collider.gameObject.GetComponentInParent<Enemy>().type == "Tank")
                            moneyCount += 50;

                        Debug.Log($"KASA: {moneyCount}");
                        ServerSend.SetPlayerMoney(id, moneyCount);
                    }
                    _hit.collider.GetComponentInParent<Enemy>().TakeDamage(50f);
                }
                
            }
        }
    }

    public void ThrowItem(Vector3 _viewDirection, string _type)
    {
        if (health <= 0)
        {
            return;
        }
        if(itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin, _type).Initialize(_viewDirection, throwForce, id);
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

            //
            if (NetworkManager.instance.getCurrentScene().ToString() == "KillHouseMap")//nazwa mapy to killhouse
                this.transform.position = new Vector3(20f, 10f, -30f); //miejsce respawnu gracza
            else 
            { //nazwa mapy to Stadium
                this.transform.position = new Vector3(-28f, 7f, 33f); //miejsce respawnu gracza
            }
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
            if(_damage == 1190f)
            {
                SetMaster(false);
            }
        }
        ServerSend.PlayerHealth(this);
    }
    private IEnumerator Respawn()
    {

        yield return new WaitForSeconds(5f);

        if (isMaster)
        {
            gameObject.GetComponent<Player>().tag = "Master";
            //gravity = 0;
           // yVelocity = 0;
        }

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this, isMaster);

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
    public void AddItem(int _value)
    {
        itemAmount += _value;
    }
    public void InteractWithObject(Vector3 _viewDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            Debug.Log($"E on {_hit.collider.gameObject}");
            if (_hit.collider.gameObject.CompareTag("AmmoBox"))
            {
                Debug.Log("In AmmoBOX");
                //dodaj X amunicji do gracza
                if (moneyCount > ammoCost)
                {
                    ServerSend.InteractedWithItem(id, 30, "AmmoBox");
                    moneyCount -= ammoCost;
                    ServerSend.SetPlayerMoney(id, moneyCount);
                }

                //zabierz x kasy od gracza
            }
        }
    }
}
