using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Level2ProgressBar : MonoBehaviour
{
    public static Level2ProgressBar Instance{get ; private set;} // will help so we dont have to keep dragging it into the inspector
    public Slider slider;
    public int TotalEnemies = 15;
    private int enemiesDefeated = 0; 

    void Awake(){
        if(Instance == null){ //looks if there are other progress bars 
            Instance = this;

            DontDestroyOnLoad(gameObject);
            Debug.Log("good");
        }else{
            Destroy(gameObject);
            Debug.Log("No good");
        }
    }
    void Start()
    {
        if(slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = TotalEnemies;
            slider.value = enemiesDefeated;
        }
    }

    public void IncrementBar(int amount)
    {
        enemiesDefeated += amount; //if an enemy is defeated, will increase the progress
        enemiesDefeated = Mathf.Clamp(enemiesDefeated, 0, TotalEnemies); //makes sure the bar doesnt go over

        if(slider != null)
        {
            slider.value = enemiesDefeated;
        }

        if(enemiesDefeated >= TotalEnemies)
        {
            Debug.Log("Level Completed"); //will show once the progress bar is full and will have a pop up or just take to the level selector
            
        }
    }
}
