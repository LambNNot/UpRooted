using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    [SerializeField]
    protected double health = 2;

    [SerializeField]
    protected float walkSpeed = 5f;

    private float recoilForce = 1f;

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

        float horizontalDir =
            Mathf.Sign(transform.position.x - attacker.position.x);

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(horizontalDir * recoilForce, recoilForce),
            ForceMode2D.Impulse
        );

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(0, collision.transform);
        }
    }
}
