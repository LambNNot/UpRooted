using UnityEngine;
using UnityEngine.SceneManagement;

public class DevResetProgress : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteKey("LevelFirstNPC");
            PlayerPrefs.Save();

            Debug.Log("Dev reset: LevelFirstNPC progress cleared.");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}