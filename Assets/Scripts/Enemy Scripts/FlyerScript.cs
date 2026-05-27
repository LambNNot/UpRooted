using UnityEngine;

public class FlyerScript : EnemyBase
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;

    [SerializeField] private Color upColor;
    [SerializeField] private Color downColor;

    [SerializeField] private float lingerTime = 0.25f;

    private SpriteRenderer sr;

    private float topY;
    private float bottomY;

    private bool isLingering = false;
    private float lingerTimer = 0f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        topY = topPoint.position.y;
        bottomY = bottomPoint.position.y;

        moveDirection = Vector3.up;
        sr.color = upColor;
    }

    protected override void Update()
    {
        if (isLingering)
        {
            lingerTimer -= Time.deltaTime;

            if (lingerTimer <= 0f)
            {
                isLingering = false;
                TurnAroundVertical();
            }

            return;
        }

        Walk();

        if (transform.position.y >= topY && moveDirection == Vector3.up)
        {
            StartLingering();
        }
        else if (transform.position.y <= bottomY && moveDirection == Vector3.down)
        {
            StartLingering();
        }

        UpdateColor();
    }

    private void TurnAroundVertical()
    {
        moveDirection = -moveDirection;
    }

    private void StartLingering()
    {
        isLingering = true;
        lingerTimer = lingerTime;
    }

    private void UpdateColor()
    {
        if (moveDirection == Vector3.up)
        {
            sr.color = upColor;
        }
        else if (moveDirection == Vector3.down)
        {
            sr.color = downColor;
        }
    }
}