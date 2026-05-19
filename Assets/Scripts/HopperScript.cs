using UnityEngine;

public class HopperScript : EnemyBase
{

    [SerializeField]
    private float jumpForce = 1f;

    [SerializeField]
    private float jumpCooldown = 1.5f;

    [SerializeField]
    private Color waitColor;

    [SerializeField]
    private Color jumpColor;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool canJump = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        if (canJump)
        {
            Jump();
        }
        if (rb.linearVelocity == Vector2.zero)
        {
            sr.color = waitColor;
        }
    }

    private void Jump()
    {
        sr.color = jumpColor;
        canJump = false;

        // Reset vertical velocity so jumps stay consistent
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );

        rb.AddForce(getJumpVector(), ForceMode2D.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }
     
    private Vector2 getJumpVector()
    {
        return new Vector2(
            getMoveDirection().x * walkSpeed,
            jumpForce
        );
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Turn around when hitting walls
        rb.linearVelocityX = 0; // Stop all horizontal force
        TurnAround();
    }
}