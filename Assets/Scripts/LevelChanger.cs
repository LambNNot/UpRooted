using UnityEngine; 
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    [SerializeField]
    private LevelConnection _connection; 

    [SerializeField]
    private string _targetSceneName; //will be for the scene you would want it to go 

    [SerializeField]
    private Transform _spawnPoint; //will be for the player on where to spawn when going to next room

    private void Start(){
        if(_connection == LevelConnection.ActiveConnection){ //if the connection is the same then it will place the player at the fixed spawn point
            GameObject player = GameObject.FindWithTag("Player");
            if(player != null){
                player.transform.position = _spawnPoint.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){
            LevelConnection.ActiveConnection = _connection; 
            SceneManager.LoadScene(_targetSceneName); // will switch to the scene 
        }

        
    }
}