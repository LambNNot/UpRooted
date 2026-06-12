using UnityEngine;
using System.Collections;

public abstract class PlayerMovementBase : MonoBehaviour
{
    protected float horizontalInput;
    protected float speed = 8f;
    protected float jumpingPower = 16f;
    protected bool isFacingRight = true;

    protected int maxJumps = 2;
    protected int jumpsRemaining;

    protected bool canDash = true;
    protected bool isDashing;
    protected Coroutine dashingCoroutine;

    protected float dashingPower = 24f;
    protected float dashingTime = 0.2f;
    protected float dashingCoolDown = 0.5f;

    protected float fastFallMultiplier = 10.0f;
    protected float maxFallSpeed = -40f;

    [SerializeField] protected float enemyBouncePower = 10f;

    protected SpriteRenderer sr;
    protected int selectedOption = 0;
    protected float originalGravityScale;

    protected Rigidbody2D rb;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Vector2 groundCheckSize = new Vector2(1f, 0.2f);
    [SerializeField] protected LayerMask groundLayer;
    protected bool wasGrounded;

    [SerializeField] protected AudioClip dashSound;

    protected PlayerCombatBase combat;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        combat = GetComponent<PlayerCombatBase>();
        sr = GetComponent<SpriteRenderer>();
        wasGrounded = IsGrounded();

        jumpsRemaining = maxJumps;
    }

    protected virtual void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (isDashing)
            return;

        HandleGroundedState();
        HandleJump();
        HandleDash();
        Flip();
    }

    protected virtual void FixedUpdate()
    {
        if (isDashing || (combat != null && combat.isRecoiling))
            return;

        HandleMovement();
        HandleFastFall();
    }

    protected virtual void HandleGroundedState()
    {
        bool grounded = IsGrounded();

        if (grounded && !wasGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        wasGrounded = grounded;
    }

    protected virtual void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpsRemaining--;
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * 0.5f
            );
        }
    }

    protected virtual void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashingCoroutine = StartCoroutine(Dash());
        }
    }

    protected virtual void HandleMovement()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * speed,
            rb.linearVelocity.y
        );
    }

    protected virtual void HandleFastFall()
    {
        if (!IsGrounded() &&
            Input.GetAxisRaw("Vertical") < 0f &&
            rb.linearVelocity.y < 0f)
        {
            float newYVelocity =
                rb.linearVelocity.y +
                Physics2D.gravity.y *
                (fastFallMultiplier - 1f) *
                Time.fixedDeltaTime;

            newYVelocity = Mathf.Max(newYVelocity, maxFallSpeed);

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                newYVelocity
            );
        }
    }

    public virtual int GetFacingDirection()
    {
        return isFacingRight ? 1 : -1;
    }

    protected virtual bool IsGrounded()
    {
        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );
    }

    protected virtual void Flip()
    {
        if ((isFacingRight && horizontalInput < 0f) ||
            (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    protected virtual IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        if (dashSound)
        {
            AudioSource.PlayClipAtPoint(dashSound, transform.position);
        }

        rb.gravityScale = 0f;

        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravityScale;
        isDashing = false;

        yield return new WaitForSeconds(dashingCoolDown);

        canDash = true;
        dashingCoroutine = null;
    }

    public virtual void BounceFromEnemy()
    {
        if (dashingCoroutine != null)
        {
            StopCoroutine(dashingCoroutine);
            dashingCoroutine = null;
        }

        isDashing = false;
        rb.gravityScale = originalGravityScale;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, enemyBouncePower);
        jumpsRemaining = maxJumps - 1;
    }

    public virtual void ResetDashCoolDown()
    {
        if (dashingCoroutine != null)
        {
            StopCoroutine(dashingCoroutine);
            dashingCoroutine = null;
        }

        canDash = true;
        isDashing = false;
        rb.gravityScale = originalGravityScale;
    }

    protected virtual void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}