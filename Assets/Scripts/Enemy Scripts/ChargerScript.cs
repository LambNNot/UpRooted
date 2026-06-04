using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerScript : EnemyBase
{
    [SerializeField] private float chargeDelay = 0.5f;
    [SerializeField] private float chargeDuration = 1.5f;
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float sightDistance = 5f;

    [SerializeField] private LayerMask targetLayers;

    [SerializeField] private Color walkColor;
    [SerializeField] private Color prepareColor;
    [SerializeField] private Color chargeColor;

    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform wallDetectionPoint;
    [SerializeField] private float wallDetectionRadius = 0.1f;

    private bool isCharging = false;
    private bool isWaiting = false;
    private float chargeTimer = 0f;

    protected override void Start()
    {
        base.Start();
        sr.color = walkColor;
    }

    private void FixedUpdate()
    {
        if (isWaiting)
        {
            return;
        }

        if (isCharging)
        {
            UpdateWallDetectionPoint();
            ChargeForward();

            if (CheckWallDetection())
            {
                stopCharging();
                return;
            }

            chargeTimer += Time.fixedDeltaTime;

            if (chargeTimer > chargeDuration)
            {
                stopCharging();
            }

            return;
        }

        Walk();
    }

    protected override void Update()
    {
        LookAhead();
    }

    private void stopCharging()
    {
        sr.color = walkColor;
        isCharging = false;
        chargeTimer = 0f;

        TurnAround();
    }

    private void beginCharging()
    {
        isCharging = true;
        isWaiting = false;
        sr.color = chargeColor;
    }

    private void PrepareCharge()
    {
        if (isWaiting || isCharging)
        {
            return;
        }

        sr.color = prepareColor;
        isWaiting = true;

        Invoke(nameof(beginCharging), chargeDelay);
    }

    private void ChargeForward()
    {
        Vector2 dir = ((Vector2)getMoveDirection()).normalized;
        rb.MovePosition(rb.position + dir * chargeSpeed * Time.fixedDeltaTime);
    }

    private void UpdateWallDetectionPoint()
    {
        Vector2 dir = ((Vector2)getMoveDirection()).normalized;

        Vector3 localPos = wallDetectionPoint.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * Mathf.Sign(dir.x);
        wallDetectionPoint.localPosition = localPos;
    }

    private bool CheckWallDetection()
    {
        return Physics2D.OverlapCircle(
            wallDetectionPoint.position,
            wallDetectionRadius,
            targetLayers
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging)
        {
            TurnAround();
        }
    }

    private void LookAhead()
    {
        Vector2 dir = ((Vector2)getMoveDirection()).normalized;

        Debug.DrawRay(
            raycastOrigin.position,
            dir * sightDistance,
            Color.red
        );

        RaycastHit2D hit = Physics2D.Raycast(
            raycastOrigin.position,
            dir,
            sightDistance,
            targetLayers
        );

        if (hit.collider != null)
        {
            PrepareCharge();
        }
    }

    public override bool TakeDamage(double damage, Transform attacker)
    {
        if (isCharging)
        {
            return false;
        }

        return base.TakeDamage(damage, attacker);
    }

    private void OnDrawGizmosSelected()
    {
        if (wallDetectionPoint == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallDetectionPoint.position, wallDetectionRadius);
    }
    protected override void TurnAround()
    {
        base.TurnAround();
        UpdateWallDetectionPoint();
    }
}



