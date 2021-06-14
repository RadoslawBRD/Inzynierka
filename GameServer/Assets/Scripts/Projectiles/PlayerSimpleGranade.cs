using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimpleGranade : ProjectileBase
{
    protected override void Start()
    {
        type = "Basic";
        base.Start();
    }
}
