using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    [SerializeField]
    protected double health = 2;

    [SerializeField]
    protected float walkSpeed = 5f;

    private Vector3 moveDirection = Vector3.left;

    protected abstract void Update();

    private void TakeDamage(double damage)
    {
        health -= Math.Floor(damage);
    }

    protected void TurnAround()
    {
        moveDirection = -moveDirection;
    }

    protected void Walk()
    {
        transform.position += moveDirection * walkSpeed * Time.deltaTime;
    }

    protected Vector3 getMoveDirection()
    {
        return moveDirection;
    }
}
