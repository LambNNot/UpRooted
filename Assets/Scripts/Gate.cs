using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [Header("Level Info")]
    public string sceneToLoad;

    [Header("Gate State")]
    public bool unlocked = true;

    private bool playerInside = false;

    void Update()
    {
        if (playerInside && unlocked)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}