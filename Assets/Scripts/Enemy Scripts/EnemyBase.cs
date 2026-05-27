using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    [SerializeField]
    protected double health = 2;

    [SerializeField]
    protected float walkSpeed = 5f;

    private float knockbackForce = 3f;

    protected Vector3 moveDirection = Vector3.left;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected abstract void Update();

    private void TakeDamage(double damage, Transform attacker)
    {
        health -= Math.Floor(damage);

        // Direction away from attacker
        Vector2 knockbackDir =
            (transform.position - attacker.position).normalized;

        // Reset current velocity so knockback feels consistent
        rb.linearVelocity = Vector2.zero;

        // Apply physics knockback
        rb.AddForce(
            knockbackDir * knockbackForce,
            ForceMode2D.Impulse
        );
    }

    protected void TurnAround()
    {
        moveDirection = -moveDirection;
        SwitchDirection();
    }

    protected void Walk()
    {
        transform.position += moveDirection * walkSpeed * Time.deltaTime;
    }

    protected Vector3 getMoveDirection()
    {
        return moveDirection;
    }

    private void SwitchDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }
}
