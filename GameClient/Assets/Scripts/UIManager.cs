using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipAddress;
    public GameObject pauseMenu;
    public bool inGamePause=false;
    //public GameObject backGround;
    //public GameObject settingsMenu;
    //public GameObject saveButton;
    //public InputField ipField;
    //private string ipAddress;

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
   
    public void ConnectedToServer()
    {
        for(int i = 0; i < usernameField.text.Length; i++)
        {
            if(usernameField.text[i].ToString() == " ")
            {
                Debug.Log("Nick nie mo¿e zawieraæ spacji");
                return;
            }
        }
        if(usernameField.text == "")
        {
            Debug.Log("Nick nie mo¿e byc pusty");
            return;
        }
        //usernameField.interactable = false;
        //Client.instance.ConnectedToServer(ipField.text);

        if (Regex.Matches(ipAddress.text, @"[a-zA-Z!@#$%^&*()_+{};':<>,?/|\s]").Count == 0)
        {
            Debug.Log("Connecting to the server");
            startMenu.SetActive(false);

            Client.instance.ConnectedToServer(ipAddress.text);
            ToggleCoursorMode();
        }
        else
            Debug.LogError("Invalid characters in IP Address");

       
       // Debug.Log(ipAddress);
    }
    public void Offlinegame()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.OfflineGame();
    }
    public void OnSettings()
    {
        usernameField.interactable = false;
        startMenu.SetActive(false);

       //ipField.interactable = true;
       // ipField.enabled = true;
        //saveButton.SetActive(true);

    }
    public void onSave()
    {
       // ipField.interactable = false;
       // saveButton.SetActive(false);
       //
       // usernameField.interactable = true;
       // startMenu.SetActive(true);
       //
       // ipAddress = Settings.instance.ipField.text;
       // Debug.Log(ipAddress);

    }
    public bool onPauseChange()
    {
        inGamePause = !inGamePause;
        return inGamePause;
    }
    public void changeInGamePauseMenu()
    {
        bool isPaused = onPauseChange();

        pauseMenu.SetActive(isPaused);
        //backGround.SetActive(isPaused);
        ToggleCoursorMode();

    }
    public void onResume()
    {
        changeInGamePauseMenu();
    }
    public void onDisconnect()
    {
       // Client.instance.tcp.Disconnect();
       // Client.instance.udp.Disconnect();
        Client.instance.Disconnect();
        startMenu.SetActive(true);
        changeInGamePauseMenu();
        ToggleCoursorMode();
        Camera.main.transform.position=new Vector3(0, 1, -10);
    }

    private void ToggleCoursorMode()
    {
        Cursor.visible = !Cursor.visible;
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            
        }
    }
    public void onExit()
    {
        Debug.Log("Game is exiting");
        Application.Quit();        
    }

}
