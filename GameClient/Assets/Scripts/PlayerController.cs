using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientSend.PlayerShoot(camTransform.forward);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClientSend.PlayerThrowItem(camTransform.forward);
        }
        if (Input.GetKeyDown(KeyCode.BackQuote)|| Input.GetKeyDown(KeyCode.Tilde))
        {
            DebugConsole.instance.OpenCloseConsole();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //UIManager.instance.changeInGamePauseMenu();
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
