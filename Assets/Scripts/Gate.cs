using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [Header("Level Info")]
    public string sceneToLoad;

    [Header("Gate State")]
    public bool unlocked = true;

    private bool playerInside = false;

    void Start()
    {
        if(unlocked == true)
        {
            GetComponent<SpriteRenderer>().color = Color.softYellow;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.grey;
        }
    }

    void Update()
    {
        if (playerInside && unlocked)
        {
            if (Input.GetKeyDown(KeyCode.E)){
                LoadNextLevel();
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
    private void LoadNextLevel()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}