using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject enemyBasicPrefab;
    public GameObject enemyTankPrefab;
    public GameObject playerSimpleGranadePrefab;
    public GameObject enemyStonePrefab;

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

    public ProjectileBase InstantiateProjectile(Transform _shootOrigin, string _type)
    {
        switch (_type)
        {
            case "Basic":
                return Instantiate(playerSimpleGranadePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<PlayerSimpleGranade>();
            case "Stone":
                return Instantiate(enemyStonePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<EnemyProjectileStone>();

            default:
                return Instantiate(playerSimpleGranadePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<PlayerSimpleGranade>();

        }
    }
    public void InstantiateEnemy(Vector3 _position, string _type)
    {
        switch (_type)
        {
            case "Basic":
                Instantiate(enemyBasicPrefab, _position, Quaternion.identity);
                break;
            case "Tank":
                Instantiate(enemyTankPrefab, _position, Quaternion.identity);
                break;
            case "Poisone":
                break;
            case "Jumper":
                break;
            default:
                Instantiate(enemyBasicPrefab, _position, Quaternion.identity);
                break;
        }
    }

}
