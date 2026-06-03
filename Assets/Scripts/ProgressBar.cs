using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public int TotalEnemies = 10;
    private int enemiesDefeated = 0; 

    void Start()
    {
        if(slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = TotalEnemies;
            slider.value = 0;
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
