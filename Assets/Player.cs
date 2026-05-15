using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public CharacterData characterD; //this will be for the character and the next 2 variables
    public SpriteRenderer artworkSprite;
    private int selectedOption = 0;
    public float moveSpeed = 5f; // these will be for the character movement
    private Vector2 screenBounds;
    private float playerHalfWidth;
    private float horizontalInput;

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedOption")) // this will check if there is a saved data or will give the player the character at 0
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

        UpdateCharacter(selectedOption);

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
    }

    private void UpdateCharacter(int selectedOption) //gets the name and character from the character data and updates it
    {
        Character character = characterD.GetCharacter(selectedOption);
        artworkSprite.sprite = character.characterSprite; 
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption"); //sets the value of selectedoption to whatever number the key is 
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");// this will be for the character input and movement
        float newX = transform.position.x + (horizontalInput * moveSpeed * Time.deltaTime); 

        float clampedX = Mathf.Clamp(newX, -screenBounds.x + playerHalfWidth, screenBounds.x - playerHalfWidth); // this will be so the player doesn't go out of bounds
        transform.position = new Vector2(clampedX, transform.position.y);
    }
}
