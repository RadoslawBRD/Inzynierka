using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipAddress;

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
        startMenu.SetActive(false);
        usernameField.interactable = false;
        //Client.instance.ConnectedToServer(ipField.text);
        Debug.Log("Connecting to the server");
        Client.instance.ConnectedToServer(ipAddress.text);
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
    public void onExit()
    {
        Debug.Log("Game is exiting");
        Application.Quit();        
    }

}
