using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public Vector3 targetDirection;

    [Header("Collision Detection")]
    [SerializeField] private RaycastHit hitInfo;
    public bool isGrounded; 

    [Header("Custom Physics")]
    [SerializeField] private float gravityModifier; 

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>(); 
    }

    private void Update()
    { 
        if (targetDirection.magnitude > 0f)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 0.1f) && hitInfo.collider.gameObject.CompareTag("Court Floor"))
            {
                isGrounded = true; 
            }
            else
            {
                isGrounded = false; 
            }
        }
    }

    private void FixedUpdate()
    {
        //if not grounded, apply gravity 
        if (!isGrounded)
        {
            targetDirection += gravityModifier * Physics.gravity;
        }
        else
            targetDirection.y = 0f; 

        //MovePosition is for kinematic Rigidbodies
        rb.position += targetDirection * Time.fixedDeltaTime; 
    }

    //https://learn.unity.com/tutorial/live-session-2d-platformer-character-controller#5c7f8528edbc2a002053b695
}
