using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool isGameLive = false;
    int playerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager is working");

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
                StartCoroutine(GameRestart(playerCount));
                Debug.Log("Game is starting...");

                
            }
            
        }
        
    }
    private IEnumerator GameRestart(int _playsers)
    {
        isGameLive = true;
        yield return new WaitForSeconds(5f);
        Debug.Log("Resetting game");
        
        int _selectedPlayer = Random.Range(1, _playsers);
        
        Server.clients[_selectedPlayer].player.SetMaster();
       

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                Debug.Log($"Killing player{_client.player.id}");
                _client.player.TakeDamage(1000f);
            }
        }
        

    }
    
}
