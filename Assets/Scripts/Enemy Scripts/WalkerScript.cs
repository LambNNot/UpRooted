using UnityEngine;

public class WalkerScript : EnemyBase
{
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float ledgeCheckDistance = 0.75f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float turnCooldown = 0.15f;
    private float turnCooldownTimer = 0f;

    private float ledgeCheckStartX;

    protected override void Start()
    {
        base.Start();

        if (ledgeCheck != null)
        {
            ledgeCheckStartX = Mathf.Abs(ledgeCheck.localPosition.x);
            UpdateLedgeCheckPosition();
        }
    }

    protected override void Update()
    {
        Walk();

        if (turnCooldownTimer > 0f)
        {
            turnCooldownTimer -= Time.deltaTime;
            return;
        }

        if (!IsGroundAhead())
        {
            TurnAround();
            turnCooldownTimer = turnCooldown;
        }
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
        UpdateLedgeCheckPosition();
    }

    private void UpdateLedgeCheckPosition()
    {
        if (ledgeCheck == null)
            return;

        Vector3 localPosition = ledgeCheck.localPosition;
        localPosition.x = moveDirection.x < 0f ? -ledgeCheckStartX : ledgeCheckStartX;
        ledgeCheck.localPosition = localPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (ledgeCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            ledgeCheck.position,
            ledgeCheck.position + Vector3.down * ledgeCheckDistance
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TurnAround();
    }
}

