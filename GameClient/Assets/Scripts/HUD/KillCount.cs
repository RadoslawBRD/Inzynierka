using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KillCount : MonoBehaviour
{
    public static KillCount instance;
    public int killCount=0;
    public int killTarget=0;
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
    public void SetKillCount(int _killCount, int _killTarget)
    {
        killCount = _killCount;
        killTarget = _killTarget;
    }
}