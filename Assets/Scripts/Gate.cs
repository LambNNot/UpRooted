using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [Header("Level Info")]
    public string sceneToLoad;

    [Header("Gate State")]
    public bool unlocked = false;

    private bool playerInside = false;
    
    public bool passprogressbar = false; //this will be so then the gate could be activated
    private SpriteRenderer spriteRenderer; 
    public ProgressBar levelProgressBar; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(passprogressbar){ // this will be for the gates in the level selector
            unlocked = true;
        }
        else{
            unlocked = false; // will be locked if its a normal level gate

            if (levelProgressBar == null && Level2ProgressBar.Instance == null) // will look for a progress bar 
            {
                levelProgressBar = FindFirstObjectByType<ProgressBar>();
            }
        }

        UpdateGateColor();
    }

    void Update()
    {
        if(!passprogressbar && !unlocked && CheckIfProgressIsFull()){ //this will activate the gate when the progressBar is full and is not a gate in the level selector
            unlocked = true; 
            UpdateGateColor();
            Debug.Log("ProgressBar is full, now the gate unlocked.");

        }

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

    private bool CheckIfProgressIsFull(){
        if(Level2ProgressBar.Instance != null){ // this will check the progress bar in the levels and the level2progressbar
            return Level2ProgressBar.Instance.slider.value >= Level2ProgressBar.Instance.TotalEnemies;
        }
        if(levelProgressBar != null){
            return levelProgressBar.slider.value >= levelProgressBar.TotalEnemies;
        }

        return false;
    }

    private void UpdateGateColor(){
        if(spriteRenderer != null){

            if(unlocked){ //this will change the color depending on the state
                spriteRenderer.color = Color.softYellow;
            }
            else{
                spriteRenderer.color = Color.grey;
            }
        }
    }
}