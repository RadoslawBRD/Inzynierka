using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCount : MonoBehaviour
{
    public static MoneyCount instance;

    public int currentMoney;
    public Text moneyDisplay;



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
    public void setMoney(int _ammount)
    {
        moneyDisplay.text = _ammount.ToString();
    }
}
