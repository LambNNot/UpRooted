using UnityEngine; 

public class Level2Room : MonoBehaviour
{
    [SerializeField] private Collider2D _doorCollider; // The exit door/level changer
    [SerializeField] private Collider2D _entryZoneCollider; // The big room entry trigger, which will be the floor within the scene

    private int _remainingEnemies; 
    private bool _roomActivated = false;
    private bool _isUnlocked = false; 

    private void Start()
    {
        if (_doorCollider != null)  //will set the trigger to true for both 
        {
            _doorCollider.isTrigger = true; 
        }

        if (_entryZoneCollider != null)
        {
            _entryZoneCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //will call to lock the door/collider
    {
        if (other.CompareTag("Player") && !_roomActivated && !_isUnlocked)
        {
            ActivateRoomLock();
        }
    }

    private void ActivateRoomLock()
    {
        _roomActivated = true;

        if (_doorCollider != null) // will check how many enemies are within the room and then will lock the player in 
            _remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        {
            _doorCollider.isTrigger = false; 
            Debug.Log("Room is locked, defeat enemies to get out");
        }

        if (_remainingEnemies == 0) //base case just in case no enemies are in the room
        {
            Unlock(); 
        }
    }

    public void EnemyDefeated()
    {
        if (!_roomActivated || _isUnlocked) return; 

        _remainingEnemies--; //will decrement to keep track of how many enemies are left
        Debug.Log("Enemies have been defeated!");

        if (_remainingEnemies <= 0)
        {
            Unlock();
        }
    }    

    private void Unlock()
    {
        _isUnlocked = true; //will open the door/enable collider so player can get out
        if (_doorCollider != null)
        {
            _doorCollider.isTrigger = true; 
            Debug.Log("Enemies defeated! The door is open.");
        }
    }
}
