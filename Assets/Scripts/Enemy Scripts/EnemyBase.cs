using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{

    [SerializeField]
    protected double health = 2;

    [SerializeField]
    protected float walkSpeed = 5f;

    private float knockbackForce = 1f;

    protected Vector3 moveDirection = Vector3.left;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    [SerializeField]
    protected AudioClip deathSound;
    [SerializeField]
    protected AudioClip hitSound;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected abstract void Update();

    public bool TakeDamage(double damage, Transform attacker)
    {
        health -= Math.Floor(damage);

        if (health <= 0)
        {
            Die();
            return true;
        }

        if (hitSound)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);    
        }
        

        float recoilForce = knockbackForce;
        if (damage > 0)
        {
            recoilForce *= 5;
        }

        float horizontalDir =
            Mathf.Sign(transform.position.x - attacker.position.x);

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(horizontalDir * recoilForce, recoilForce),
            ForceMode2D.Impulse
        );

        return false;

    }

    private void Die()
    {
        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);   
        }
        ProgressBar progressBar = FindAnyObjectByType<ProgressBar>(); //these lines will increment the bar whenever an enemy has died
        if(progressBar != null)
        {
            progressBar.IncrementBar(1);
        }

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
    protected virtual void ApplyKnockback(Transform attacker, float recoilForce)
    {
        float horizontalDir =
            Mathf.Sign(transform.position.x - attacker.position.x);

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(horizontalDir * recoilForce, recoilForce),
            ForceMode2D.Impulse
        );
    }
    

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //child enemy overrides if they need collision behavior 
    }

}
