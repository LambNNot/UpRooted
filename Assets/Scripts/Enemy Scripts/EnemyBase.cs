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

    private bool isDead = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected abstract void Update();

    public virtual bool TakeDamage(double damage, Transform attacker)
    {
        if (isDead)
            return false;

        health -= Math.Floor(damage);

        if (health <= 0)
        {
            isDead = true;
            Die();
            return true;
        }

        if (hitSound)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        if (damage > 0)
        {
            float recoilForce = knockbackForce * 5;
            ApplyKnockback(attacker, recoilForce);
        }

        return false;
    }

    private void Die()
    {
        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);   
        }
        ProgressBar progressBar = FindAnyObjectByType<ProgressBar>(); //these lines will increment the bar whenever an enemy has died for levels that are one scene
        if(progressBar != null)
        {
            progressBar.IncrementBar(1);
        }

        Level2ProgressBar progressbar = FindAnyObjectByType<Level2ProgressBar>(); // these lines will be for levels that have multiple rooms
        if(progressbar != null){
            progressbar.IncrementBar(1);
        }

        Level2Room roomLock = FindAnyObjectByType<Level2Room>();
        if (roomLock != null)
        {
            roomLock.EnemyDefeated();
        }

        Destroy(gameObject);
    }

    protected virtual void TurnAround()
    {
        moveDirection = -moveDirection;
        SwitchDirection();
        OnTurnAround();
    }

protected virtual void OnTurnAround()
{
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
        sr.flipX = !sr.flipX;
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
    

    // protected virtual void OnCollisionEnter2D(Collision2D collision)
    // {
    //     //child enemy overrides if they need collision behavior 
    // }

}
