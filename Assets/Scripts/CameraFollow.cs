using UnityEngine;

// CameraFollow.cs
// Resolves Issue #29: "Camera is bound by enemies on screen"
//
// THE BUG:
//   The camera was getting "stuck" on the left side of enemies, preventing the
//   player from moving past them. This happened because the Camera GameObject
//   had picked up physics components (Rigidbody2D / Collider2D) somewhere along
//   the way, so it was physically colliding with enemy colliders and refusing
//   to move forward.
//
// THE FIX:
//   1. The camera transform is updated directly via SmoothDamp (already was),
//      which is the correct approach. We keep that.
//   2. On Awake we proactively strip any Rigidbody2D / Collider2D / Collider
//      components that might have been added to the camera in the Inspector.
//      This makes the script self-healing and defends against the bug coming
//      back if someone re-adds those components by accident.
//   3. We also expose the offset and smoothTime as Inspector fields so they
//      can be tuned per-scene without code changes.

[DisallowMultipleComponent]
public class CameraFollow : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target; // the player

    [Header("Follow Settings")]
    [SerializeField]
    [Tooltip("Offset from the target position. Z should stay negative so the camera looks at the scene.")]
    private Vector3 offset = new Vector3(0f, 1.6f, -10f);

    [SerializeField]
    [Tooltip("How long (in seconds) it takes the camera to catch up to the player. Lower = snappier.")]
    private float smoothTime = 0.25f;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        StripPhysicsComponents();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    /// <summary>
    /// Removes any physics components from the camera so it cannot be blocked
    /// by enemy colliders. The camera should be a pure visual follower; it has
    /// no business participating in the physics simulation.
    /// </summary>
    private void StripPhysicsComponents()
    {
        // Remove 2D physics (the most likely culprit for this game).
        Rigidbody2D rb2d = GetComponent<rigidbody2d>();
        if (rb2d != null)
        {
            Debug.LogWarning("CameraFollow: Removed Rigidbody2D from camera. " +
                             "The camera should not have physics components.");
            Destroy(rb2d);
        }

        Collider2D[] colliders2D = GetComponents();
        for (int i = 0; i < colliders2D.Length; i++)
        {
            Debug.LogWarning("CameraFollow: Removed " + colliders2D[i].GetType().Name +
                             " from camera. The camera should not have colliders.");
            Destroy(colliders2D[i]);
        }

        // Also defend against 3D physics components, just in case.
        Rigidbody rb = GetComponent();
        if (rb != null) Destroy(rb);

        Collider[] colliders = GetComponents();
        for (int i = 0; i < colliders.Length; i++)
        {
            Destroy(colliders[i]);
        }
    }
}