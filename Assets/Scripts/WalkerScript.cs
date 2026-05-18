using System;
using UnityEngine;

public class WalkerScript : EnemyBase
{

    // Update is called once per frame
    protected override void Update()
    {
        Walk(); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        moveDirection = -moveDirection;
    }

}
