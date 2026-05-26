using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;


public class MainMenu : MonoBehaviour
{
    public string NewGameScene; // this will be to input scene the player will go once button is pressed
    public string HelpOrLoreScene; // this will be to input the scene that will take the player to the lore or help scene

    public void NewGame()
    {
        SceneManager.LoadScene(NewGameScene);
    }

    public void HelpOrLore()
    {
        SceneManager.LoadScene(HelpOrLoreScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
        
}
