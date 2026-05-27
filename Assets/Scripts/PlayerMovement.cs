using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private double health = 3;

    private float horizontalInput;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCoolDown = 0.5f; 

    private float recoilForce = 8f;

    private float fastFallMultiplier = 10.0f; // fast fall mechanics 
    private float maxFallSpeed = -40f;
    public CharacterData characterD; //this will be for the character and the next 2 variables
    private SpriteRenderer sr;
    private int selectedOption = 0;

    private bool isRecoiling = false;

    [SerializeField]
    private float recoilDuration = 0.2f;
    [SerializeField]
    private Color recoilColor;
    private Color originalColor;


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        if (!PlayerPrefs.HasKey("selectedOption")) //this will check if there is a saved data or will give the player the character at 0
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selectedOption) // gets the name and character from the character data and updates it
    {
        Character character = characterD.GetCharacter(selectedOption);
        sr.sprite = character.characterSprite;
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption"); //sets the value of selectedOption to whatever number the key is
    }

    void Update()
    {
        if(isDashing)
            return;
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing || isRecoiling)
            return;

        rb.linearVelocity = new Vector2(
            horizontalInput * speed,
            rb.linearVelocity.y
        );

        // Fast fall
        if (!IsGrounded() &&
            Input.GetAxisRaw("Vertical") < 0f &&
            rb.linearVelocity.y < 0f)
        {
            float newYVelocity =
                rb.linearVelocity.y +
                Physics2D.gravity.y *
                (fastFallMultiplier - 1f) *
                Time.fixedDeltaTime;

            newYVelocity = Mathf.Max(newYVelocity, maxFallSpeed);

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                newYVelocity
            );
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }

    private void TakeDamage(Transform attacker)
    {
        health -= 1;

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
    
}