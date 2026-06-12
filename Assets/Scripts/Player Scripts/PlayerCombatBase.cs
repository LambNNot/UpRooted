using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public abstract class PlayerCombatBase : MonoBehaviour
{
    [SerializeField] protected int health = 3;
    [SerializeField] protected AudioClip hitSound;

    public HealthBar healthBar;

    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected GameObject attackHitbox;
    [SerializeField] protected Collider2D attackHitboxCollider;

    protected float attackRange = 0.7f;
    protected float attackOffset = 1.225f;

    [SerializeField] protected InputActionReference attackAction;

    protected Vector2 lastAttackDirection = Vector2.zero;

    protected float recoilForce = 8f;
    protected float recoilDuration = 0.2f;

    [SerializeField] protected Color recoilColor;
    protected Color originalColor;

    public bool isRecoiling { get; protected set; } = false;

    protected float attackCooldown = 0.25f;
    protected float hitboxVisibleTime = 0.1f;
    protected Color hitboxColor = Color.blue;

    public bool isAttacking;
    public bool isInvulnerable;

    protected bool canAttack = true;

    protected GameObject currentHitboxVisual;

    protected SpriteRenderer sr;
    protected Rigidbody2D rb;
    protected PlayerMovementBase playerMovement;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovementBase>();
        originalColor = sr.color;

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        StartCoroutine(HealthBarReset());
    }

    protected virtual void Update()
    {
        lastAttackDirection = GetAttackDirection();
    }

    protected virtual IEnumerator HealthBarReset()
    {
        yield return new WaitForEndOfFrame();

        if (healthBar == null)
        {
            healthBar = FindAnyObjectByType<HealthBar>();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
            Debug.Log("Full Health");
        }
        else
        {
            Debug.Log("Not full health");
        }
    }

    protected virtual void TakeDamage(Transform attacker)
    {
        if (isInvulnerable)
            return;

        health -= 1;

        PlayHitSound();
        UpdateHealthBar();
        ApplyRecoil(attacker);

        StartCoroutine(RecoilCoroutine());

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void PlayHitSound()
    {
        if (hitSound)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
    }

    protected virtual void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }
    }

    protected virtual void ApplyRecoil(Transform attacker)
    {
        float horizontalDir =
            Mathf.Sign(transform.position.x - attacker.position.x);

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(horizontalDir * recoilForce, recoilForce),
            ForceMode2D.Impulse
        );
    }

    protected virtual IEnumerator RecoilCoroutine()
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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isRecoiling)
        {
            Debug.Log("Player hit enemy: " + collision.gameObject.name);
            TakeDamage(collision.transform);
        }
    }

    public virtual void Die()
    {
        Debug.Log("Player died");
        StartCoroutine(RespawnRoutine());
    }

    protected virtual IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (Level2ProgressBar.Instance != null)
        {
            Level2ProgressBar.Instance.IncrementBar(
                -Level2ProgressBar.Instance.TotalEnemies
            );

            Destroy(Level2ProgressBar.Instance.gameObject);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Level2Room")
        {
            SceneManager.LoadScene("LevelSecond");
        }
        else
        {
            SceneManager.LoadScene(currentSceneName);
        }
    }

    protected virtual void Attack(Vector2 inputDirection)
    {
        if (!canAttack)
            return;

        BeginAttack();

        PositionAttackHitbox(inputDirection);
        StartCoroutine(EnableAttackHitbox(inputDirection));
    }

    protected virtual IEnumerator EnableAttackHitbox(Vector2 inputDirection)
    {
        if (attackHitbox == null || attackHitboxCollider == null)
        {
            EndAttack();
            yield break;
        }

        attackHitbox.SetActive(true);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            attackHitbox.transform.position,
            attackRange
        );

        foreach (Collider2D collider in colliders)
        {
            HandleAttackCollider(collider, inputDirection);
        }

        yield return new WaitForSeconds(hitboxVisibleTime);

        attackHitbox.SetActive(false);
        EndAttack();
    }

    protected virtual void PositionAttackHitbox(Vector2 inputDirection)
    {
        if (attackHitbox == null)
            return;

        Vector2 direction = inputDirection;

        if (direction == Vector2.zero)
            direction = GetAttackDirection();

        Vector2 worldOffset = direction.normalized * attackOffset;

        attackHitbox.transform.position =
            (Vector2)transform.position + worldOffset;
    }

    protected virtual void BeginAttack()
    {
        isAttacking = true;
        isInvulnerable = true;
        StartCoroutine(AttackCooldown());
    }

    protected virtual void EndAttack()
    {
        isInvulnerable = false;
        isAttacking = false;
    }

    protected virtual Vector2 GetAttackCenter(Vector2 inputDirection)
    {
        Vector2 attackCenter = transform.position;

        if (inputDirection != Vector2.zero)
        {
            attackCenter += inputDirection.normalized * attackOffset;
        }

        return attackCenter;
    }

    protected virtual void HandleAttackCollider(
        Collider2D collider,
        Vector2 inputDirection
    )
    {
        if (!collider.CompareTag("Enemy"))
            return;

        EnemyBase enemyBase = collider.GetComponent<EnemyBase>();

        if (enemyBase == null)
            return;

        bool hitFromAbove =
            inputDirection.y < 0f &&
            transform.position.y > collider.transform.position.y;

        bool enemyDied = enemyBase.TakeDamage(1, transform);

        if (hitFromAbove && playerMovement != null)
        {
            playerMovement.BounceFromEnemy();

            if (enemyDied)
            {
                playerMovement.ResetDashCoolDown();
            }
        }
    }

    protected virtual IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    protected virtual IEnumerator ShowHitboxVisual(Vector2 position)
    {
        GameObject hitbox = new GameObject("AttackHitbox");

        SpriteRenderer hitboxRenderer =
            hitbox.AddComponent<SpriteRenderer>();

        hitboxRenderer.sortingOrder = 10;
        hitboxRenderer.sprite = CreateCircleSprite();
        hitboxRenderer.color = hitboxColor;

        hitbox.transform.position = position;
        hitbox.transform.localScale = Vector3.one * attackRange * 2f;

        yield return new WaitForSeconds(hitboxVisibleTime);

        Destroy(hitbox);
    }

    protected virtual Sprite CreateCircleSprite()
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

    protected virtual void OnDrawGizmosSelected()
    {
        if (attackHitbox != null)
        {
            Gizmos.DrawWireSphere(attackHitbox.transform.position, attackRange);
        }
    }

    protected virtual void OnEnable()
    {
        attackAction.action.performed += OnAttack;
        attackAction.action.Enable();
    }

    protected virtual void OnDisable()
    {
        attackAction.action.performed -= OnAttack;
        attackAction.action.Disable();
    }

    protected virtual void OnAttack(InputAction.CallbackContext context)
    {
        Attack(GetAttackDirection());
    }

    protected virtual Vector2 GetAttackDirection()
    {
        Vector2 inputDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (inputDirection.y > 0f)
            return Vector2.up;

        if (inputDirection.y < 0f)
            return Vector2.down;

        int facingDirection = 1;

        if (playerMovement != null)
        {
            facingDirection = playerMovement.GetFacingDirection();
        }

        return new Vector2(facingDirection, 0f);
    }
}