using UnityEngine;

public class SquidcapMovement : PlayerMovementBase
{
    protected override void Start()
    {
        base.Start();
        isFacingRight = false;
    }
}
