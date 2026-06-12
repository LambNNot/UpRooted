using UnityEngine;

public class SquidcapCombat : PlayerCombatBase
{

    [SerializeField] Animator animator;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnAttackStarted(Vector2 inputDirection)
{
    if (inputDirection.y < 0f)
    {
        animator.SetInteger("attackDirection", 2);
    }
    else if (inputDirection.y > 0f)
    {
        animator.SetInteger("attackDirection", 1);
    }
    else
    {
        animator.SetInteger("attackDirection", 0);
    }

    animator.SetTrigger("attack");
}
}
