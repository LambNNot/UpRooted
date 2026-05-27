using UnityEngine;

public class CollectNPC : MonoBehaviour
{
    [SerializeField] private string npcID = "LevelFirstNPC";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerIsNear = false;

    private void Start()
    {
        if (PlayerPrefs.GetInt(npcID, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(interactKey))
        {
            RescueNPC();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = true;
            Debug.Log("Press E to rescue NPC.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    private void RescueNPC()
    {
        PlayerPrefs.SetInt(npcID, 1);
        PlayerPrefs.Save();

        Debug.Log(npcID + " rescued.");

        gameObject.SetActive(false);
    }
}