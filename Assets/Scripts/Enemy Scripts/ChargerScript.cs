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

    private SpriteRenderer sr;

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
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = walkColor;

        Debug.Log("=== Charger Started ===");
        Debug.Log("Target Layer Mask Value: " + targetLayers.value);
        Debug.Log("Sight Distance: " + sightDistance);
        Debug.Log("Charge Speed: " + chargeSpeed);
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

        UpdateSpriteDirection();

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging)
        {

            stopCharging();

            return;
        }


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

    private void UpdateSpriteDirection()
    {
        Vector3 scale = transform.localScale;

        // Looking right
        if (getMoveDirection().x > 0)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        // Looking left
        else if (getMoveDirection().x < 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }
}