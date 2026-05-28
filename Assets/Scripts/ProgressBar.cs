using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ProgressBar : MonoBehaviour
{
    public Slider slider;

    public int totalEnemies = 10;
    private int enemiesDefeated = 0;


    void Start()
    {
        if(slider != null)  //will give the slider a default value of 0 
        {
            slider.minValue = 0;
            slider.maxValue = totalEnemies;
            slider.value = 0;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) // for testing, the progress will go up when pressing backspace, will change later 
        {
            IncrementBar(1);
        }        
    }

    public void IncrementBar(int amount)
    {
        enemiesDefeated += amount; //if an enemy is defeated, will increase the progress 
        enemiesDefeated = Mathf.Clamp(enemiesDefeated, 0, totalEnemies); //makes sure it doesn't go over

        if (slider != null)
        {
            slider.value = enemiesDefeated;
        }

        if (enemiesDefeated >= totalEnemies)
        {
            Debug.Log("Level Completed"); //will show once the progress bar is full and will have a pop up or just take to the level selector
        }
    }


}
