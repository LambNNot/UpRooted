using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 offset = new Vector3(0f, 1.6f, -10f);
    private float smoothTime = 0.25f; // time to reach target
    private Vector3 velocity = Vector3.zero; 
    [SerializeField] private Transform target; // the player that the camera will follow 
    
    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
