using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameLive = false;
    public int killTarget = 3;
    public int killCount = 0;
    public int _selectedPlayer; //id gracza, kt�ry jest masterem
    int playerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager is working");

    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("instance already exists, destroying object!");
            Destroy(this);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(killTarget <= killCount)
        {
            killCount = 0;
            StartCoroutine(RestartGame(0.1f));
            Debug.Log("Koniec gry!");
        }
        /*playerCount = 0;
        foreach (Client _client in Server.clients.Values)
        {

            if(_client.player != null)
            {
                playerCount++;
            }
        }
        */ 
        if (!isGameLive)
        {
            if (playerCount >= 2)
            {
                StartCoroutine(SelectMaster());
                Debug.Log("Game is starting...");
                Debug.Log("Game would be starting...");
            }

        }
        
    }
    private IEnumerator SelectMaster()
    {
        playerCount = 0;
        foreach (Client _client in Server.clients.Values)
        {

            if (_client.player != null)
            {
                playerCount++;
            }
        }
        isGameLive = true;
        Debug.Log("Resetting game");

        _selectedPlayer = 1;// UnityEngine.Random.Range(1, playerCount);
        
        Server.clients[_selectedPlayer].player.SetMaster(true);
        Server.clients[_selectedPlayer].player.itemAmount = 0;

        yield return new WaitForSeconds(5f);

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                _client.player.itemAmount = 0;
                Debug.Log($"Killing player{_client.player.id}");
                _client.player.TakeDamage(1000f);
            }
        }

        foreach (Enemy _enemy in Enemy.enemies.Values.ToList())
        {
            if (_enemy.health > 0)
            {
                _enemy.TakeDamage(_enemy.maxHealth + 1000f);
                Debug.Log($"Zabijam zombie{ _enemy.id}");
            }
        }
    }
    private IEnumerator RestartGame(float seconds)
    {
        isGameLive = false;
        try
        {            
            Server.clients[_selectedPlayer].player.itemAmount = 0;
            Server.clients[_selectedPlayer].player.SetMaster(false);
            _selectedPlayer = 100;
        }
        catch(Exception _ex)
        {
            Debug.Log(_ex);
        }

        Debug.Log("Resetting game");
         

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                Debug.Log($"Killing player{_client.player.id}");
                _client.player.TakeDamage(1190f); // przesy�am informacje o resecie przez zadawnie obra�e�
            }
        }

        
        foreach(Enemy _enemy in Enemy.enemies.Values.ToList())
        {
            if (_enemy.health > 0)
            {
                _enemy.TakeDamage(_enemy.maxHealth+1000f);
                Debug.Log($"Zabijam zombie{ _enemy.id}");
            }
        }

        Enemy[] enemyInScene = FindObjectsOfType<Enemy>();
        foreach (Enemy x in enemyInScene)
        {
            x.TakeDamage(x.maxHealth + 1000f);
            Debug.Log("Dobijam zombie");
        }

            yield return new WaitForSeconds(seconds);
        Debug.Log("Retarting game");

    }

    public void UpdateKillScore()
    {
        if (isGameLive)
        {
            killCount++;
            ServerSend.KillTargetUpdate(killCount, killTarget);
        }
            

    }
    public void runRestartGame(float seconds)
    {
        StartCoroutine(RestartGame(seconds));

    }



    //////////// commands/////////////
    public void startGame()
    {
        StartCoroutine(SelectMaster());
    }
    public void restartGame()
    {
        StartCoroutine(RestartGame(5f));

    }
    public void killAllEnemies()
    {
        foreach (Enemy _enemy in Enemy.enemies.Values.ToList())
        {
            if (_enemy.health > 0)
            {
                _enemy.TakeDamage(_enemy.maxHealth + 100f);
            }
        }
    }
    public void addGranade(int _toPlayer, string _howMany)
    {
        Server.clients[_toPlayer].player.AddItem(int.Parse(_howMany));
    }

    public void setKillTarget(int _killTarget)
    {
        killTarget = _killTarget;

    }
    public void set_map(string _map)
    {
        NetworkManager.instance.Set_map(_map.ToString());
        ServerSend.ChangeMap(_map);
    }


}
