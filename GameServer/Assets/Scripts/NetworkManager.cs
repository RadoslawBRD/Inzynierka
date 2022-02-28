using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject enemyBasicPrefab;
    public GameObject enemyTankPrefab;
    public GameObject playerSimpleGranadePrefab;
    public GameObject enemyStonePrefab;
    private string currrentScene = "StadiumMap"; // domyœlna   StadiumMap   KillHouseMap

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
       

        SceneManager.LoadScene(currrentScene, LoadSceneMode.Additive);

        Server.Start(6, 1000); // Server.Start(maxPlayers, Port)
    }
    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    public Player InstantiatePlayer()
    {
        if (NetworkManager.instance.getCurrentScene().ToString() == "KillHouse")//nazwa mapy to killhouse
            return Instantiate(playerPrefab, new Vector3(-13f, 10f, 25f), Quaternion.identity).GetComponent<Player>(); //zwraca referencje do playera / miejsce spawnu gracza
        else
         //nazwa mapy to Stadium
            return Instantiate(playerPrefab, new Vector3(-28f, 7f, 33f), Quaternion.identity).GetComponent<Player>(); //zwraca referencje do playera / miejsce spawnu gracza
                                                                                                                      //miejsce spawnu gracza
        
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
    public void Set_map(string _map)
    {

        switch (_map)
        {
            case "KillHouseMap":
                try
                {
                    currrentScene = "KillHouseMap";
                    SceneManager.UnloadScene("StadiumMap");
                    SceneManager.UnloadScene("KillHouseMap"); //TODO: zmieniæ na UnloadSceneAsync jak przygotujemy loading screen
                    
                }
                catch { }
                SceneManager.LoadScene("KillHouseMap", LoadSceneMode.Additive);
                break;
            case "StadiumMap":
                try
                {
                    currrentScene = "StadiumMap";

                    SceneManager.UnloadScene("KillHouseMap"); //TODO: zmieniæ na UnloadSceneAsync jak przygotujemy loading screen
                    SceneManager.UnloadScene("StadiumMap");
                }
                catch { }
                
                SceneManager.LoadScene("StadiumMap", LoadSceneMode.Additive);
                break;
            default:
                break;
        }
        GameManager.instance.runRestartGame(0.1f);

    }
    public string getCurrentScene()
    {
        return currrentScene;
    }

}
