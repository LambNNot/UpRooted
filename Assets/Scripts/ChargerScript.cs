using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private bool isCharging = false;
    private bool isWaiting = false;
    private float chargeTimer = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = walkColor;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isWaiting) {
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
        sr.color = prepareColor;
        isWaiting = true;
        StartCoroutine(WaitCharge());
        
    }

    private void ChargeForward()
    {
        transform.position += moveDirection * chargeSpeed * Time.deltaTime;
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
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            moveDirection,
            sightDistance,
            targetLayers
        );

        if (hit.collider != null)
        {
            Debug.Log("Saw: " + hit.collider.gameObject.name);
            PrepareCharge();
        }
    }
}
