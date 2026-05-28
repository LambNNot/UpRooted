using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerScript : EnemyBase
{
    [SerializeField]
    private float chargeDelay = 0.5f;

    [SerializeField]
    private float chargeDuration = 1.5f;

    [SerializeField]
    private float chargeSpeed = 15f;

    [SerializeField]
    private float sightDistance = 5f;

    [SerializeField]
    private LayerMask targetLayers;

    [SerializeField]
    private Color walkColor;

    [SerializeField]
    private Color prepareColor;

    [SerializeField]
    private Color chargeColor;

    [SerializeField]
    private List<GameObject> targets = new List<GameObject>();

    [SerializeField]
    private Transform raycastOrigin;

    private bool isCharging = false;
    private bool isWaiting = false;
    private float chargeTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        sr.color = walkColor;
    }

    // Update is called once per frame
    protected override void Update()
    {

        if (isWaiting)
        {
            return;
        }

        if (isCharging)
        {
            ChargeForward();

            chargeTimer += Time.deltaTime;


            if (chargeTimer > chargeDuration)
            {
                stopCharging();
            }

            return;
        }

        Walk();

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

    private IEnumerator WaitCharge()
    {

        yield return new WaitForSeconds(chargeDelay);


        beginCharging();
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

        Vector3 movement = dir * chargeSpeed * Time.deltaTime;

        transform.position += movement;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging)
        {

            stopCharging();

            return;
        }
        base.OnCollisionEnter2D(collision);
        TurnAround();
    }

    private void LookAhead()
    {
        Vector2 dir = ((Vector2)getMoveDirection()).normalized;

        Debug.DrawRay(
            transform.position,
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

    
}