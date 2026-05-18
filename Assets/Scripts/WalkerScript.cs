using System;
using UnityEngine;

public class WalkerScript : MonoBehaviour
{
    [SerializeField]    
    private float moveSpeed = 5f; // Units per second
    private Vector3 moveDirection = Vector3.left;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Walk(); 
    }

    private void Walk()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        moveDirection = -moveDirection;
    }


}
