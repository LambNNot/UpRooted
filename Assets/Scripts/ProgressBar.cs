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
        if(slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = totalEnemies;
            slider.value = 0;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            IncrementBar(1);
        }        
    }

    public void IncrementBar(int amount)
    {
        enemiesDefeated += amount;
        enemiesDefeated = Mathf.Clamp(enemiesDefeated, 0, totalEnemies);

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
