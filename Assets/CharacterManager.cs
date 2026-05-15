using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine.SceneManagement; 

public class CharacterManager : MonoBehaviour
{
    public CharacterData characterD; 
    public TextMeshProUGUI nameText;
    public SpriteRenderer artworkSprite;
    private int selectedOption = 0; 

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedOption"))// this will check if there is a saved data or will give the player the character at 0
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

        UpdateCharacter(selectedOption);
    }

    public void NextOption() // this will be for the next button, to look through the playable characters
    {
        selectedOption ++;

        if(selectedOption >= characterD.CharacterCount)
        {
            selectedOption = 0;
        }

        UpdateCharacter(selectedOption);
    }

    public void BackOption() //this will just go in reverse of the next option
    {
        selectedOption--;

        if(selectedOption < 0)
        {
            selectedOption = characterD.CharacterCount - 1;
        }

        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selectedOption) //gets the name and character from the character data and updating it
    {
        Character character = characterD.GetCharacter(selectedOption);
        artworkSprite.sprite = character.characterSprite; 
        nameText.text = character.characterName;
    }

    private void Load() // sets the value of selectedOption to whatever number the key is
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

    private void Save() // stores selectedoption value into the key 
    {
        PlayerPrefs.SetInt("selectedOption", selectedOption);
    }

    public void ChangeScene(int sceneID) //this will be to change the scene
    {
        SceneManager.LoadScene(sceneID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
