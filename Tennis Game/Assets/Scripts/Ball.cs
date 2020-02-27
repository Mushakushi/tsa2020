using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public bool isMoving; 
    public Vector3 velocity;

    [Space]
    public Vector3 bounceVelocity;
    public float bounceHieghtMultiplier;
    public float bounceDistanceMultiplier; 

    [Header("Collision Detection")]
    public bool isGrounded;
    [SerializeField] private RaycastHit hitInfo;
    [SerializeField] private float length;
    [SerializeField] private float shellRadius; 

    [Header("Gravity")]
    [SerializeField] private float gravityModifier; 

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //not moving until proven otherwise 
        isMoving = false; 
        //grounded is false until proven otherwise 
        isGrounded = false;
        //if we not being slapped by a racket fall; if we're being hit, freeze (the computations...)!
        velocity = !isMoving ? velocity /*are we bouncing?*/ + gravityModifier * Physics.gravity : Vector3.zero;
        if (!isMoving)
        {
            velocity += gravityModifier * Physics.gravity; 
        }
        //crude way of falling back down after bounce velocity is done affecting us 
        
        //if we're falling naturally, do some calculations to get us falling and colliding 
        if (velocity.magnitude > 0.001f && !isMoving)
        {
            float distance = velocity.magnitude; //length of direction that we are trying to move 
            Physics.Raycast(transform.position, Vector3.down, out hitInfo, length);
            //if we're hitting the ground 
            if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Court Floor"))
            {
                //normal (how the line reflects) of the ground surface
                Vector3 groundNormal = hitInfo.normal;
                isGrounded = true; 

                //distance to the collider 
                float modifiedDistance = hitInfo.distance - shellRadius;
                //if we're about to move into a collider adjust our velocity to not hit it 
                distance = distance > modifiedDistance ? modifiedDistance : distance; 
            }
            
            //if we're not being moved by the tennis racket, then move according to normal gravity and physics bruh 
            if (!isMoving) rb.position += velocity.normalized /*sheer direction*/ * distance /*"magnitude"*/ * Time.fixedDeltaTime /*smooth*/;
        }

        //an adapted version of Unity's kinematic Rigidbody Platfomer Character Controller tutorial 
        //https://learn.unity.com/tutorial/live-session-2d-platformer-character-controller#5c7f8528edbc2a002053b695
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, length, 0)); 
    }
}
