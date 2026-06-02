using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    
    [SerializeField]
    private int health = 3;
    public HealthBar healthBar; // will be for the slider

    private float attackRange = 0.55f;
    private float attackOffset = 1.0f;

    
    [SerializeField] private InputActionReference attackAction;

    private Vector2 lastAttackDirection = Vector2.zero;
    
    private float recoilForce = 8f;
    private float recoilDuration = 0.2f;
    
    [SerializeField]
    private Color recoilColor;
    private Color originalColor;

    
    public bool isRecoiling { get; private set; } = false;

    private float attackCooldown = 0.25f;
    private float hitboxVisibleTime = 0.1f;
    private Color hitboxColor = Color.blue;

    public bool isAttacking;
    public bool isInvulnerable;

    private bool canAttack = true;

    private GameObject currentHitboxVisual;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        if(healthBar != null) //will set the bar to max
        {
            healthBar.SetMaxHealth(health);
        }
    }

    private void Update()
    {
        Vector2 inputDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        lastAttackDirection = inputDirection.normalized;

    }

    private void TakeDamage(Transform attacker)
    {
        if (isInvulnerable)
        {
            return;
        }

        health -= 1;

        if(healthBar != null) // will update the slider
        {
            healthBar.SetHealth(health);
        }

        float horizontalDir =
            Mathf.Sign(transform.position.x - attacker.position.x);

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(horizontalDir * recoilForce, recoilForce),
            ForceMode2D.Impulse
        );

        StartCoroutine(RecoilCoroutine());

        if (health <= 0)
        {
            Die();
            
        }
    }

    private IEnumerator RecoilCoroutine()
    {
        isRecoiling = true;

        float elapsed = 0f;
        float flashInterval = 0.08f;

        while (elapsed < recoilDuration)
        {
            sr.color = recoilColor;
            yield return new WaitForSeconds(flashInterval);

            sr.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2f;
        }

        sr.color = originalColor;
        isRecoiling = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isRecoiling)
        {
            Debug.Log("Player hit enemy: " + collision.gameObject.name);
            TakeDamage(collision.transform);
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
    }
    private void Attack(Vector2 inputDirection)
    {
        if (!canAttack)
            return;


        isAttacking = true;
        isInvulnerable = true;
        StartCoroutine(AttackCooldown());

        Vector2 attackCenter = transform.position;

        if (inputDirection != Vector2.zero)
        {
            attackCenter += inputDirection.normalized * attackOffset;
        }

        StartCoroutine(ShowHitboxVisual(attackCenter));

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            attackCenter,
            attackRange
        );

        foreach (Collider2D collider in colliders)
        {
            if (!collider.CompareTag("Enemy"))
                continue;

            EnemyBase enemyBase = collider.GetComponent<EnemyBase>();

            if (enemyBase != null)
            {
                enemyBase.TakeDamage(1, transform);
            }
        }

        isInvulnerable = false;
        isAttacking = false;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    private IEnumerator ShowHitboxVisual(Vector2 position)
    {
        GameObject hitbox = new GameObject("AttackHitbox");

        SpriteRenderer hitboxRenderer =
            hitbox.AddComponent<SpriteRenderer>();

        hitboxRenderer.sprite = CreateCircleSprite();
        hitboxRenderer.color = hitboxColor;

        hitbox.transform.position = position;
        hitbox.transform.localScale =
            Vector3.one * attackRange * 2f;

        yield return new WaitForSeconds(hitboxVisibleTime);

        Destroy(hitbox);
    }

    private Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(128, 128);

        Color[] colors = new Color[128 * 128];

        Vector2 center = new Vector2(64, 64);
        float radius = 60f;

        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                float distance =
                    Vector2.Distance(new Vector2(x, y), center);

                colors[y * 128 + x] =
                    distance <= radius
                        ? Color.white
                        : Color.clear;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, 128, 128),
            new Vector2(0.5f, 0.5f)
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackCenter = transform.position;

        if (lastAttackDirection != Vector2.zero)
        {
            attackCenter += lastAttackDirection * attackOffset;
        }

        Gizmos.DrawWireSphere(attackCenter, attackRange);
    }
    private void OnEnable()
    {
        attackAction.action.performed += OnAttack;
        attackAction.action.Enable();
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttack;
        attackAction.action.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        Attack(inputDirection);
    }
}
