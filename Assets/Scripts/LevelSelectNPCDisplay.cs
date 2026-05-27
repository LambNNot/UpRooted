using UnityEngine;

public class LevelSelectNPCDisplay : MonoBehaviour
{
    [SerializeField] private string npcID = "LevelFirstNPC";
    [SerializeField] private GameObject npcVisual;

    private void Start()
    {
        int savedValue = PlayerPrefs.GetInt(npcID, 0);

        Debug.Log("LevelSelectNPCDisplay running on " + gameObject.name);
        Debug.Log("Checking " + npcID + ". Saved value = " + savedValue);

        if (npcVisual == null)
        {
            Debug.LogWarning("NPC Visual is not assigned on " + gameObject.name);
            return;
        }

        npcVisual.SetActive(savedValue == 1);

        Debug.Log("NPC Visual active state set to: " + npcVisual.activeSelf);
    }
}