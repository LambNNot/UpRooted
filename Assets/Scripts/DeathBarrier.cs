using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// DeathBarrier.cs
// Resolves Issue #32: death barrier for level 1
//
// USAGE (for teammates working in Unity):
// 1. Create an empty GameObject in your level scene (name it "DeathBarrier")
// 2. Add a BoxCollider2D component to it
// 3. Check "Is Trigger" on the BoxCollider2D
// 4. Stretch the collider wide enough to span the entire level width
//    (extra wide so the player can't "miss" it)
// 5. Position it below the lowest reachable point in the level
// 6. Attach this DeathBarrier script to the GameObject
// 7. Set "Level Select Scene Name" in the inspector (e.g. "LevelSelector")
// 8. (Optional) Save it as a prefab in Assets/Prefabs for reuse across levels

public class DeathBarrier : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField]
    [Tooltip("The exact name of the level select scene to return to when the player dies.")]
    private string levelSelectSceneName = "LevelSelector";

    [Header("Death Settings")]
    [SerializeField]
    [Tooltip("Small delay before scene transition (lets death effects play if added later).")]
    private float deathDelay = 0.1f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("Player hit death barrier. Returning to level select: " + levelSelectSceneName);
            StartCoroutine(KillPlayer());
        }
    }

    private IEnumerator KillPlayer()
    {
        // Small delay so we can later hook in screen-fade, sound effects, etc.
        // (matches the "screen fades to black" behavior described in the design doc)
        yield return new WaitForSeconds(deathDelay);

        if (string.IsNullOrEmpty(levelSelectSceneName))
        {
            Debug.LogError("DeathBarrier: levelSelectSceneName is not set in the Inspector!");
            yield break;
        }

        SceneManager.LoadScene(levelSelectSceneName);
    }

    // Visualize the death barrier in the Scene view as a translucent red box
    // so designers can see it even when not selected.
    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<boxcollider2d>();
        if (box == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Vector3 center = transform.position + (Vector3)box.offset;
        Vector3 size = new Vector3(
            box.size.x * transform.lossyScale.x,
            box.size.y * transform.lossyScale.y,
            0.1f
        );
        Gizmos.DrawCube(center, size);
    }
}