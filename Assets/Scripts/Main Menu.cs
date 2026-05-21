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

    public void NewGame()
    {
        SceneManager.LoadScene(NewGameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
        
}
