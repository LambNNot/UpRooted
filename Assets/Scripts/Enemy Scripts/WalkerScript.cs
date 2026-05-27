using System;
using UnityEngine;

public class WalkerScript : EnemyBase
{

    // Update is called once per frame
    protected override void Update()
    {
        Walk(); 
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        TurnAround();
    }

}
