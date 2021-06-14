using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthCount : MonoBehaviour
{
    public static HealthCount instance;

    public float currentHP;
    public float maxHp;
    public Text healthDisplay;

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
   
    public void SetHealth(float _health)
    {
        healthDisplay.text = _health.ToString("0.0");
    }

}
