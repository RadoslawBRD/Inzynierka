using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DebugCommandBase;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole instance;

    bool showConsole=false;
    string input;

    public static DebugCommand START_GAME;
    public static DebugCommand RESTART_GAME;
    public static DebugCommand KILL_ALL_ENEMIES;
    public static DebugCommand<string> ADD_GRANADE;
    

    public List<object> commandList;

    public void Update()
    {
        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            Debug.Log($"czytam komende{input}");

            HandleInput();
            input = "";
        }

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
        START_GAME = new DebugCommand("start_game", "Choose master, start game", "start_game", () =>
         {
             Debug.Log("sending star_game");
             ClientSend.PlayerSentCommand("start_game",null);
         });
        RESTART_GAME = new DebugCommand("restart_game", "Restart game", "restart_game", () =>
        {
            Debug.Log($"sending {input}");
            ClientSend.PlayerSentCommand("restart_game", null);
        });
        KILL_ALL_ENEMIES = new DebugCommand("kill_all_enemies", "Restart game", "kill_all_enemies", () =>
        {
            Debug.Log("sending kill_all_enemies");
            ClientSend.PlayerSentCommand("kill_all_enemies", null);
        });
        ADD_GRANADE = new DebugCommand<string>("add_granade", "Add granade", "add_granade <int>", (x) =>
        {
            Debug.Log("sending add_granade");
            ClientSend.PlayerSentCommand("add_granade", x);
        });

        commandList = new List<object>
        {
            START_GAME,
            RESTART_GAME,
            KILL_ALL_ENEMIES,
            ADD_GRANADE
        };
    }

    public void OpenCloseConsole()
    {
        showConsole = !showConsole;
    }

    private void OnGUI()
    {
        if (!showConsole)
            return;
        float y = 2f;
        //Debug.Log($"Console is: {showConsole}");
        GUI.Box(new Rect(0, y, Screen.width, 30), "");

        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }
    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if(input.Equals(commandBase.commandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<string> != null)
                    {
                        (commandList[i] as DebugCommand<string>).Invoke(properties[1]);
                    }
                
            }
        }
    }


}
