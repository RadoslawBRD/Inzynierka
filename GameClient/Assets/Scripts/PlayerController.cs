using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;
    public static Dictionary<int, string> selectedThrowable = new Dictionary<int, string>();


    private void Start()
    {
        selectedThrowable[1] = "Basic";
        selectedThrowable[2] = "Stone";
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, camTransform.forward, Color.red, 100f,true);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(BulletsCount.instance.bulletsCurrent>0)
                ClientSend.PlayerShoot(camTransform.forward);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClientSend.PlayerThrowItem(camTransform.forward,selectedThrowable[2]);
        }
        if (Input.GetKeyDown(KeyCode.BackQuote)|| Input.GetKeyDown(KeyCode.Tilde))
        {
            DebugConsole.instance.OpenCloseConsole();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.changeInGamePauseMenu();
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
            Input.GetKey(KeyCode.Space)
        };

        ClientSend.PlayerMovement(_inputs);

    }
}
