using UnityEngine;

public class SquidcapMovement : PlayerMovementBase
{

    [SerializeField] Animator animator;

    protected override void Start()
    {
        base.Start();
        isFacingRight = false;
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

     protected override void Update()
    {
        base.Update();

        bool isMoving =
            Mathf.Abs(horizontalInput) > 0.01f &&
            !isDashing &&
            (combat == null || !combat.isRecoiling);

        animator.SetBool("isMoving", isMoving);
    }

    protected override void OnJump()
    {
        animator.SetTrigger("jump");
    }
}
