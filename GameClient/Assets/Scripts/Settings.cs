using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{

    public static Settings instance;
    public InputField ipField;
    // Start is called before the first frame update
    void Start()
    {
        
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

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public string getIP()
    {
        Debug.Log(ipField.ToString());

        return ipField.ToString();

    }

}
