using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>(); //przechowuje informacje o graczach po stronie klienta
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();//s³ownik przechowuj¹cy spawnery
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();//s³ownik przechowuj¹cy bomby/obiekty rzucane
    public static Dictionary<int, EnemyManager> enemies = new Dictionary<int, EnemyManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject simplePlayerGranadePrefab;
    public GameObject enemyStonePrefab;
    public GameObject enemyBasicPrefab;
    public GameObject enemyTankPrefab;
    public GameObject offlinePlayer;
    
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

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if(_id == Client.instance.myId)//sprawdza czy pojawiaj¹cy siê gracz jest graczem lokalnym, jesli tak, to odpowiednio przypisjemy prefaby
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }
    public void OfflineSpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId)//sprawdza czy pojawiaj¹cy siê gracz jest graczem lokalnym, jesli tak, to odpowiednio przypisjemy prefaby
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }
    public void CreateItemSapwner(int _spawnerId, Vector3 _position, bool _hasItem)
    {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId,_hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }
    public void SpawnProjectile(int _id, Vector3 _position, string _type){
        GameObject _projectile = null;
        switch (_type)
        {
            case "Basic":
                _projectile = Instantiate(simplePlayerGranadePrefab, _position, Quaternion.identity);
                break;
            case "Stone":
                _projectile = Instantiate(enemyStonePrefab, _position, Quaternion.identity);
                break;
            case "Poisone":
                break;
            case "Jumper":
                break;
            default:
                _projectile = Instantiate(simplePlayerGranadePrefab, _position, Quaternion.identity);
                break;                               
        }
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }
    public void SpawnEnemy(int _id, float _maxhelath, Vector3 _position, string _type)
    {
        GameObject _enemy=null;
        switch (_type)
        {
            case "Basic":
                _enemy = Instantiate(enemyBasicPrefab, _position, Quaternion.identity);
                break;
            case "Tank":
                _enemy = Instantiate(enemyTankPrefab, _position, Quaternion.identity);
                break;
            case "Poisone":
                break;
            case "Jumper":
                break;
            default:
                _enemy = Instantiate(enemyBasicPrefab, _position, Quaternion.identity);
                break;

        }
        _enemy.GetComponent<EnemyManager>().Initialize(_id, _maxhelath);
        enemies.Add(_id, _enemy.GetComponent<EnemyManager>());
    }
    public void DisconnectFromServer()
    {
        try
        {
            SceneManager.UnloadScene("Main");
        }
        catch { }
        try { players.Clear(); } catch { }
        try { enemies.Clear(); } catch { }
        try { itemSpawners.Clear(); } catch { }
        try { projectiles.Clear(); } catch { }
        //Destroy(gameObject);
        try
        {
            SceneManager.LoadScene("Main",LoadSceneMode.Additive);
        }
        catch { }
        Debug.Log("zamykanko");
    }

}

