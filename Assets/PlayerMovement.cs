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

    public SpriteRenderer artworkSprite;
    private int selectedOption = 0;
    public CharacterData characterD; //this will be for the character and the next 2 variables
    private float fastFallMultiplier = 10.0f; // fast fall mechanics 
    private float maxFallSpeed = -40f;


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

        Flip();
    }

    private void FixedUpdate(){
    if (isDashing)
        return;

    rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

    // Fast fall when pressing down while in the air
    if (!IsGrounded() && Input.GetAxisRaw("Vertical") < 0f && rb.linearVelocity.y < 0f)
    {
        float newYVelocity = rb.linearVelocity.y + Physics2D.gravity.y * (fastFallMultiplier - 1f) * Time.fixedDeltaTime;

        // Prevent falling infinitely fast
        newYVelocity = Mathf.Max(newYVelocity, maxFallSpeed);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, newYVelocity);
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
}