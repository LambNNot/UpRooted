using UnityEngine;

public class HopperScript : EnemyBase
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float horizontalJumpForce = 3f;
    [SerializeField] private float jumpCooldown = 1.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Ledge Check")]
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float ledgeCheckDistance = 0.75f;

    [Header("Colors")]
    [SerializeField] private Color waitColor;
    [SerializeField] private Color jumpColor;

    private bool canJump = true;
    private float ledgeCheckStartX;

    [SerializeField] private Transform player;
    [SerializeField] private float playerDetectionRange = 5f;
    [SerializeField] private bool chasePlayer = true;

    private float groundCheckStartX;

    protected override void Start()
    {
        base.Start();

        sr.color = waitColor;

        if (ledgeCheck != null)
        {
            ledgeCheckStartX = Mathf.Abs(ledgeCheck.localPosition.x);
            UpdateLedgeCheckPosition();
        }
        if (groundCheck != null)
        {
            groundCheckStartX = Mathf.Abs(groundCheck.localPosition.x);
            UpdateGroundCheckPosition();
        }
    }

    protected override void Update()
    {
        if (!IsGrounded())
            return;

        sr.color = waitColor;

        if (!canJump)
            return;
        
        FacePlayerIfNearby();

        if (!IsGroundAhead())
        {
            TurnAround();
            return;
        }

        Jump();
    }

    private void Jump()
    {
        sr.color = jumpColor;
        canJump = false;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(GetJumpVector(), ForceMode2D.Impulse);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private Vector2 GetJumpVector()
    {
        return new Vector2(
            getMoveDirection().x * horizontalJumpForce,
            jumpForce
        );
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );
    }

    private bool IsGroundAhead()
    {
        if (ledgeCheck == null)
            return true;

        return Physics2D.Raycast(
            ledgeCheck.position,
            Vector2.down,
            ledgeCheckDistance,
            groundLayer
        );
    }

    protected override void OnTurnAround()
    {
        UpdateGroundCheckPosition();
        UpdateLedgeCheckPosition();
    }
    private void UpdateLedgeCheckPosition()
    {
        if (ledgeCheck == null)
            return;

        Vector3 localPosition = ledgeCheck.localPosition;
        localPosition.x = getMoveDirection().x < 0f ? -ledgeCheckStartX : ledgeCheckStartX;
        ledgeCheck.localPosition = localPosition;
    }
    private void UpdateGroundCheckPosition()
    {
        if (groundCheck == null)
            return;

        Vector3 localPosition = groundCheck.localPosition;
        localPosition.x = getMoveDirection().x < 0f ? -groundCheckStartX : groundCheckStartX;
        groundCheck.localPosition = localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                TurnAround();
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (ledgeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                ledgeCheck.position,
                ledgeCheck.position + Vector3.down * ledgeCheckDistance
            );
        }
    }

    private void FacePlayerIfNearby()
    {
        if (!chasePlayer || player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > playerDetectionRange)
            return;

        float xDifference = player.position.x - transform.position.x;

        if (Mathf.Abs(xDifference) < 0.1f)
            return;

        float directionToPlayer = Mathf.Sign(xDifference);

        if (directionToPlayer != Mathf.Sign(getMoveDirection().x))
        {
            TurnAround();
        }
    }
}