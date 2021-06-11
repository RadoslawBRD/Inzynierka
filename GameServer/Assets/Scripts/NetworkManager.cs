using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject projectilePrefab;

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
    public void Start()
    {
        QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60; //target frame rate

        Server.Start(6, 1000); // Server.Start(maxPlayers, Port)
    }
    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f,0.5f,0f), Quaternion.identity).GetComponent<Player>(); //zwraca referencje do playera
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }
    public void InstantiateEnemy(Vector3 _position)
    {
        Instantiate(enemyPrefab, _position, Quaternion.identity);
    }

}
