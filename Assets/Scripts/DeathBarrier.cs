using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// DeathBarrier.cs
// Resolves Issue #32: death barrier for level 1
//
// USAGE (for teammates working in Unity):
// 1. Create an empty GameObject in your level scene (name it "DeathBarrier")
// 2. Add a BoxCollider2D component to it
// 3. Check "Don't panic — the file isn't lost. Two things happened:

1. **YouIs Trigger" on the BoxCollider2D
// 4. Stretch the collider wide enough to span the entire level width
//    (make it extra wide so the player can't "miss" it)
// 5. Position it below the lowest re're on `main` branch right now**, not on `32-death-barrier`. The DeathBarrier file only exists on that branch (and on GitHub).
2. **The file you pushed was actually empty** — that's why the commit said `achable point in the level
// 6. Attach this DeathBarrier script to the GameObject
// 7. Set "Level Select Scene Name" in the inspector to your level select scene name
// 8. (Optional) Save it as a prefab in Assets/Prefabs so it can be reused across all levels

public class DeathBarrier : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField]
    [Tooltip("The exact name of the level select scene to return to when the player dies.")]
    private string levelSelectS0 insertions(+), 0 deletions(-)`. You created `DeathBarrier.cs` in VS Code but never saved any content into it (or saved it empty).

Let me prove both points and then we'll fix it.

## Step 1: Confirm the file exists on theceneName = "Level branSelectorch but";

    [Header("Death is empty

```bash Sett
git checings")]
    [Serkout ializeField]
    [32-death-barrierTooltip("Small delay before scene transition (l
ls -la Assets/Scripts/DeathBetsarrier.cs
w deathc -l Assets/ effects play if added later).")]
    private float deathDelay = 0.Scripts/DeathB1arrier.cs
```f;

    private

You b'ool hasTriggered = false;

    ll see the file exists but has 0 lines. That's the bug.

## Step 2: Now actually put the code in the file

While still on `32-death-barrier`, open `Assets/Scripts/DeathBarrier.cs` in VS Code and paste this **complete, corrected**private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("Player hit death barrier. Returning to level select: " + levelSelectSceneName);
            StartCoroutine(KillPlayer());
        }
    }

    private IEnumerator KillP code (I fixed the `BoxCollider2D` typo too):

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// DeathBarrier.cs
// Resolves Issue #32layer()
    {
        // Small delay so we can later hook in screen-fade, sound effects, etc.
        // (matches the "screen fades to black" behavior described in the design doc)
        yield return new WaitForSecon: death barrier for level 1
//
// USAGE (for teammates working in Unity):
// 1. Create an empty GameObject in your level scene (name it "DeathBarrier")
// 2. Add a BoxCollider2D componentds(deathDelay);

        if (string.IsNullOrEmpty(levelSelectSceneName))
        {
            Debug.LogError("DeathBarrier: levelSelectSceneName is not set in the Inspector!");
            yield break;
        }

        SceneManager.LoadScene(levelSelectSceneName);
    }

    // Vis to it
// 3. Check "Is Trigger" on the BoxCollider2D
// 4. Stretch the collider wide enough to span the entire level width
//    (extra wide so the player can't "miss" it - perualize the death barrier in the Scene view so designers can see it clearly,
    // even when the BoxCollider2D gizmo isn't selected.
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        BoxCollider2D box = GetComponent<boxcollider2d> LeilaGroves's comment)
// 5. Position it below the lowest reachable point in the level
// 6. Attach this DeathBarrier script to the GameObject
// 7. Set "Level Select();
        if (box != null)
        {
            Vector3 center = transform.position + (Vector3)box.offset;
            Vector3 size = new Vector3(box.size.x * transform.lossyScale.x,
                                       box.size.y * transform.lossyScale.y,
                                       0.1f);
            Gizmos.DrawCube(center, size);
        }
    }
}
