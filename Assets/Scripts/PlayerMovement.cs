using UnityEngine;
using System.Collections;

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
    private float dashingCoolDown = 0.5f; 

    private float fastFallMultiplier = 10.0f; // fast fall mechanics 
    private float maxFallSpeed = -40f;
    public CharacterData characterD; //this will be for the character and the next 2 variables
    private SpriteRenderer sr;
    private int selectedOption = 0;



    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1f, 0.2f);
    [SerializeField] private LayerMask groundLayer;

    private PlayerCombat combat;

    void Start()
    {
        
        combat = GetComponent<PlayerCombat>();
        sr = GetComponent<SpriteRenderer>();
        // if (!PlayerPrefs.HasKey("selectedOption")) //this will check if there is a saved data or will give the player the character at 0
        // {
        //     selectedOption = 0;
        // }
        // else
        // {
        //     Load();
        // }
        // UpdateCharacter(selectedOption);
        Debug.Log($"Player active: {gameObject.activeInHierarchy}");
        Debug.Log($"Renderer enabled: {sr.enabled}");
        Debug.Log($"Sprite: {sr.sprite}");
        Debug.Log($"Color: {sr.color}");
        Debug.Log($"Material: {sr.sharedMaterial}");
        Debug.Log($"Shader: {sr.sharedMaterial.shader.name}");
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Scale: {transform.localScale}");
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
        if (isDashing || combat.isRecoiling)
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
        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );
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
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
    
}