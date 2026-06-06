using UnityEngine;

public class ChargerScript : EnemyBase
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeDelay = 0.5f;
    [SerializeField] private float chargeDuration = 1.0f;
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float sightDistance = 5f;

    [Header("Detection Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Colors")]
    [SerializeField] private Color walkColor;
    [SerializeField] private Color prepareColor;
    [SerializeField] private Color chargeColor;

    [Header("Detection Points")]
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform wallDetectionPoint;
    [SerializeField] private float wallDetectionRadius = 0.1f;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float ledgeCheckDistance = 0.75f;

    private bool isCharging = false;
    private bool isWaiting = false;
    private float chargeTimer = 0f;

    private float raycastOriginStartX;
    private float wallDetectionStartX;
    private float ledgeCheckStartX;

    protected override void Start()
    {
        base.Start();

        sr.color = walkColor;

        if (raycastOrigin != null)
            raycastOriginStartX = Mathf.Abs(raycastOrigin.localPosition.x);

        if (wallDetectionPoint != null)
            wallDetectionStartX = Mathf.Abs(wallDetectionPoint.localPosition.x);

        if (ledgeCheck != null)
            ledgeCheckStartX = Mathf.Abs(ledgeCheck.localPosition.x);

        UpdateDetectionPointPositions();
    }

    private void FixedUpdate()
    {
        if (isWaiting)
            return;

        if (isCharging)
        {
            ChargeForward();

            chargeTimer += Time.fixedDeltaTime;

            if (CheckWallDetection() || !IsGroundAhead())
            {
                StopCharging(true);
                return;
            }

            if (chargeTimer > chargeDuration)
            {
                StopCharging(false);
            }

            return;
        }

        if (!IsGroundAhead())
        {
            TurnAround();
            return;
        }

        Walk();
    }

    protected override void Update()
    {
        if (!isCharging && !isWaiting)
        {
            LookAhead();
        }
    }

    private void PrepareCharge()
    {
        if (isWaiting || isCharging)
            return;

        sr.color = prepareColor;
        isWaiting = true;

        Invoke(nameof(BeginCharging), chargeDelay);
    }

    private void BeginCharging()
    {
        isCharging = true;
        isWaiting = false;
        chargeTimer = 0f;
        sr.color = chargeColor;
    }

    private void StopCharging(bool shouldTurnAround)
    {
        sr.color = walkColor;
        isCharging = false;
        chargeTimer = 0f;

        if (shouldTurnAround)
        {
            TurnAround();
        }
    }

    private void ChargeForward()
    {
        Vector2 dir = ((Vector2)getMoveDirection()).normalized;
        rb.MovePosition(rb.position + dir * chargeSpeed * Time.fixedDeltaTime);
    }

    private bool CheckWallDetection()
    {
        if (wallDetectionPoint == null)
            return false;

        return Physics2D.OverlapCircle(
            wallDetectionPoint.position,
            wallDetectionRadius,
            wallLayer
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

    private void LookAhead()
    {
        if (raycastOrigin == null)
            return;

        Vector2 dir = ((Vector2)getMoveDirection()).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            raycastOrigin.position,
            dir,
            sightDistance,
            playerLayer
        );

        if (hit.collider != null)
        {
            PrepareCharge();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging && !collision.gameObject.CompareTag("Player"))
        {
            TurnAround();
        }
    }

    public override bool TakeDamage(double damage, Transform attacker)
    {
        if (isCharging)
            return false;

        return base.TakeDamage(damage, attacker);
    }

    protected override void TurnAround()
    {
        base.TurnAround();
        UpdateDetectionPointPositions();
    }

    private void UpdateDetectionPointPositions()
    {
        float directionSign = moveDirection.x < 0f ? -1f : 1f;

        if (raycastOrigin != null)
        {
            Vector3 localPos = raycastOrigin.localPosition;
            localPos.x = raycastOriginStartX * directionSign;
            raycastOrigin.localPosition = localPos;
        }

        if (wallDetectionPoint != null)
        {
            Vector3 localPos = wallDetectionPoint.localPosition;
            localPos.x = wallDetectionStartX * directionSign;
            wallDetectionPoint.localPosition = localPos;
        }

        if (ledgeCheck != null)
        {
            Vector3 localPos = ledgeCheck.localPosition;
            localPos.x = ledgeCheckStartX * directionSign;
            ledgeCheck.localPosition = localPos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (wallDetectionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wallDetectionPoint.position, wallDetectionRadius);
        }

        if (ledgeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                ledgeCheck.position,
                ledgeCheck.position + Vector3.down * ledgeCheckDistance
            );
        }

        if (raycastOrigin != null)
        {
            Gizmos.color = Color.blue;

            Vector3 dir = Application.isPlaying
                ? getMoveDirection().normalized
                : Vector3.left;

            Gizmos.DrawLine(
                raycastOrigin.position,
                raycastOrigin.position + dir * sightDistance
            );
        }
    }
}