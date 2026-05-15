using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string NewGameScene; //this will be to input which scene the player will go once button is pressed


    public void NewGame()
    {
        SceneManager.LoadScene(NewGameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}