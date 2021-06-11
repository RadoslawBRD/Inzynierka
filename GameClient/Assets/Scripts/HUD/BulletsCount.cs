using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BulletsCount : MonoBehaviour
{
    public static BulletsCount instance;

    // Start is called before the first frame update
    public int bulletsCurrent;
    public int bulletsMax;


    public Text ammoDisplay;
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


    private void OnGUI()
    {
        GUI.Box(new Rect(0, Screen.height, Screen.width, 30), "");

    }
    void Start()
    {


    }
    
    // Update is called once per frame
    void Update()
    {
        ammoDisplay.text = bulletsCurrent.ToString() + " / "  + bulletsMax.ToString();
    }
    public void updateCurrentBulets(int value)
    {
        bulletsCurrent = value;
    }public void updateMaxBulets(int value)
    {
        bulletsMax = value;
    }
}
