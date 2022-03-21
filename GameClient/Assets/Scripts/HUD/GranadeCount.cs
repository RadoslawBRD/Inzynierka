using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GranadeCount : MonoBehaviour
{
    public static GranadeCount instance;


    public int currentGranade=0;
    public Text granadeDisplay;



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

    public void setGranadeAmount(int _value)
    {
        if(currentGranade > 0 || _value > 0)
            currentGranade += _value;
        if (_value == -100) //usuwa wszystkie granaty
            currentGranade = 0;
        updateGranadeCount(currentGranade);
    }
    public void updateGranadeCount(int _count)
    {
        granadeDisplay.text = _count.ToString();

    }
}
