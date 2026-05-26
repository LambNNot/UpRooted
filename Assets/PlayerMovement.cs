using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private float horizontalInput;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCoolDown = 1f; 

    public CharacterData characterD; //this will be for the character and the next 2 variables
    public SpriteRenderer artworkSprite;
    private int selectedOption = 0;

    public int maxHealth = 5; //will be for health of the player
    public int currentHealth;

    public HealthBar healthBar;



    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedOption")) //this will check if there is a saved data or will give the player the character at 0
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(selectedOption);

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void UpdateCharacter(int selectedOption) // gets the name and character from the character data and updates it
    {
        Character character = characterD.GetCharacter(selectedOption);
        artworkSprite.sprite = character.characterSprite;
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

        if (Input.GetKeyDown(KeyCode.Return)) //for testing right now
        {
            TakeDamage(1);
        }

        Flip();
    }

    private void FixedUpdate()
    
    {
        if(isDashing)
            return;
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
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

    void TakeDamage(int damage) //this will decreas the health on the health bar 
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
}