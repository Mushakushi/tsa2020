using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public Vector3 targetDirection;

    [Header("Collision Detection")]
    public bool isGrounded;
    [SerializeField] private Vector3 groundPos;
    [SerializeField] private RaycastHit hitInfo;
    [SerializeField] private float distance;
    [SerializeField] private float shellRadius; 

    [Header("Gravity")]
    public bool applyGravity = true; 
    [SerializeField] private float gravityModifier; 

    private void Start()
    {
        applyGravity = true; 
        rb = gameObject.GetComponent<Rigidbody>(); 
    }

    private void Update()
    { 
        
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        if (applyGravity) targetDirection += gravityModifier * Physics.gravity;

        if (targetDirection.magnitude > 0f)
        {
            Physics.Raycast(transform.position, Vector3.down, out hitInfo, distance);
            if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Court Floor"))
            {
                groundPos = hitInfo.point + Vector3.up * shellRadius;
                isGrounded = true;
            }
        }

        bool isBelowGround = rb.position.y < groundPos.y;
        targetDirection.y = isBelowGround ? 0f : targetDirection.y;

        rb.position += targetDirection * Time.fixedDeltaTime;
        rb.position = isBelowGround ? new Vector3(rb.position.x, groundPos.y, rb.position.z) : rb.position; 
    }

    public Vector3 GetMovement(bool isMoving)
    {
        Vector3 targetDirection = Vector3.zero;
        if (isMoving)
        {

        }
        return targetDirection; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, distance, 0)); 
    }

    //https://learn.unity.com/tutorial/live-session-2d-platformer-character-controller#5c7f8528edbc2a002053b695
}
