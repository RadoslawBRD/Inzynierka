using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingNickname : MonoBehaviour
{

    public static FloatingNickname instance;

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
    public void Destroy()
    {
        Destroy(gameObject);
    }
    void Start()
    {
    }

}
