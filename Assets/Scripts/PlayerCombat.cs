using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    
    [SerializeField]
    private int health = 3;
    [SerializeField]
    private AudioClip hitSound;
    public HealthBar healthBar; // will be for the slider

    [SerializeField]
    private Transform attackPoint;

    private float attackRange = 0.7f;
    private float attackOffset = 1.225f;

    
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
    private PlayerMovement playerMovement; // reference to the player's movement script for the bounce mechanic

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>(); // this will be for the bounce mechanic when the player jumps on an enemy
        originalColor = sr.color;

        StartCoroutine(HealthBarReset()); //will reset the player health after dying or 
    }

    private IEnumerator HealthBarReset(){ // this will be for the health bar reset when the player dies
        yield return new WaitForEndOfFrame();


        if(healthBar == null){
            healthBar = FindAnyObjectByType<HealthBar>();
        }

        if(healthBar != null) //will set the bar to max
        {
            healthBar.SetMaxHealth(health);
            Debug.Log("Full Health");
        }else{
            Debug.Log("Not full health");
        }

    }

    private void Update()
    {
        lastAttackDirection = GetAttackDirection();
    }

    // player takes damage when colliding with an enemy 
    private void TakeDamage(Transform attacker)
    {
        if (isInvulnerable)
        {
            return;
        }

        health -= 1;
        if (hitSound)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);   
        }

        if (healthBar != null) // will update the slider
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
        StartCoroutine(RespawnRoutine()); // this will spawn the player at the start of the level 
    }

    private IEnumerator RespawnRoutine(){ // will spawn the player in level if dead 
        yield return new WaitForSeconds(.1f);

        if (Level2ProgressBar.Instance != null) // this will make sure that the progress bar resets, since the DontDestroyOnLoad was preventing it from reseting
        {
            Level2ProgressBar.Instance.IncrementBar(-Level2ProgressBar.Instance.TotalEnemies);
            Destroy(Level2ProgressBar.Instance.gameObject);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;

        if(currentSceneName == "Level2Room"){
            SceneManager.LoadScene("LevelSecond");
        }else{
            SceneManager.LoadScene(currentSceneName);

        }
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
                bool hitFromAbove = inputDirection.y < 0f && transform.position.y > collider.transform.position.y; // Check if the attack is coming from above
                bool enemyDied = enemyBase.TakeDamage(1, transform);
                if (hitFromAbove && playerMovement != null)
                {
                    playerMovement.BounceFromEnemy(); // Call the bounce method on the player's movement script
                    if (enemyDied)
                    {
                        playerMovement.ResetDashCoolDown(); // Resetting cooldown if the enemy dies from the bounce attack
                    }
                }
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
        Attack(GetAttackDirection());
    }

    private Vector2 GetAttackDirection()
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
