using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameLive = false;
    public int _selectedPlayer; //id gracza, który jest masterem
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
        playerCount = 0;
        foreach (Client _client in Server.clients.Values)
        {

            if(_client.player != null)
            {
                playerCount++;
            }
        }
        
        if (!isGameLive)
        {
            if (playerCount>=2)
            {
                //StartCoroutine(SelectMaster(playerCount));
                //Debug.Log("Game is starting...");                
                //Debug.Log("Game would be starting...");                
            }

        }
        
    }
    private IEnumerator SelectMaster(int _playsers)
    {
        isGameLive = true;
        yield return new WaitForSeconds(5f);
        Debug.Log("Resetting game");
        
        _selectedPlayer = Random.Range(1, _playsers);
        
        Server.clients[_selectedPlayer].player.SetMaster(true);
        Server.clients[_selectedPlayer].player.itemAmount = 0;


        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                Debug.Log($"Killing player{_client.player.id}");
                _client.player.TakeDamage(1000f);
            }
        }
    }
    private IEnumerator RestartGame(int _playsers)
    {
        isGameLive = false;
        Server.clients[_selectedPlayer].player.SetMaster(false);
        Server.clients[_selectedPlayer].player.itemAmount = 0;
        _selectedPlayer = 100;

        yield return new WaitForSeconds(5f);
        Debug.Log("Resetting game");

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                Debug.Log($"Killing player{_client.player.id}");
                _client.player.TakeDamage(1190f); // przesy³am kod przez zadawnie obra¿eñ
            }
        }

        foreach(Enemy _enemy in Enemy.enemies.Values.ToList())
        {
            

            if (_enemy.health > 0)
            {
                _enemy.TakeDamage(_enemy.maxHealth+100f);
            }
        }

    }
   
    //////////// commands/////////////
    public void startGame()
    {
        StartCoroutine(SelectMaster(playerCount));
    }
    public void restartGame()
    {
        StartCoroutine(RestartGame(playerCount));

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

}
