using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;
    public static Dictionary<int, string> selectedThrowable = new Dictionary<int, string>();
    public static Dictionary<int, string> selectedEnemy = new Dictionary<int, string>();
    public int selectedEnemyInt = 1;
    private string playerState="Idle";
    private void Start()
    {
        selectedThrowable[1] = "Basic";
        selectedThrowable[2] = "Stone";

        selectedEnemy[1] = "Basic";
        selectedEnemy[2] = "Tank";
    }
    private void Update()
    {
        //Debug.DrawRay(transform.position, camTransform.forward, Color.red, 100f,true);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(gameObject.tag == "Master")
            {
                ClientSend.PlayerShoot(camTransform.forward, selectedEnemy[selectedEnemyInt]);

            }
            else if(BulletsCount.instance.bulletsCurrent>0)
                ClientSend.PlayerShoot(camTransform.forward, selectedEnemy[selectedEnemyInt]);

        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClientSend.PlayerThrowItem(camTransform.forward,selectedThrowable[1]);
        }
        if (Input.GetKeyDown(KeyCode.BackQuote)|| Input.GetKeyDown(KeyCode.Tilde))
        {
            Debug.Log("KONSOLE OTWIERAM####################");
            DebugConsole.instance.OpenCloseConsole();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.changeInGamePauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && gameObject.tag == "Master")
        {
            UIManager.instance.setSelectedEnemy(1);
            selectedEnemyInt = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && gameObject.tag == "Master")
        {
            UIManager.instance.setSelectedEnemy(2);
            selectedEnemyInt = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && GetComponentInParent<GameObject>().tag == "Master")
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && GetComponentInParent<GameObject>().tag == "Master")
        {

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ClientSend.InteractWithItem(camTransform.forward);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangePlayerAnimator("Run");
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            ChangePlayerAnimator("Idle");
        }
    }
    private void FixedUpdate()
    {
        SendIntputToServer();
    }
    private void SendIntputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.E)
        };

        ClientSend.PlayerMovement(_inputs, selectedEnemy[selectedEnemyInt], playerState);

    }
    private void ChangePlayerAnimator(string _state)
    {
        switch (_state)
        {
            case "Idle":
                playerState = "Idle";
                break;
            case "Run":
                playerState = "Run";
                break;
            case "Jump":
                break;
            default:
                break;
        }
    }
}
