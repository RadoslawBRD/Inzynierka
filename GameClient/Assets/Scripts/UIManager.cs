using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;

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
        
        startMenu.SetActive(false);
        usernameField.interactable = false;
        //Client.instance.ConnectedToServer(ipField.text);
        Client.instance.ConnectedToServer("127.0.0.1");
       // Debug.Log(ipAddress);
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

}
