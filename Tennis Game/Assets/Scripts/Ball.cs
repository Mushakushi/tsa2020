﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public bool isMoving; 
    public Vector3 velocity;
    public Vector3 previousPos; 

    [Space]
    [SerializeField] private float minBounceVelocity; 
    [SerializeField] private float bounceHieghtMultiplier;
    [SerializeField] private float bounceDistanceMultiplier;

    [Header("Collision Detection")]
    public bool isGrounded;
    [SerializeField] private RaycastHit hitInfo;
    [SerializeField] private float length;
    [SerializeField] private float shellRadius; 

    [Header("Gravity")]
    [SerializeField] private float gravityModifier; 

    private void Start()
    {
        //not moving until proven otherwise 
        isMoving = false;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(transform.position.y < 0f)
        {
            rb.position = Vector3.up; 
        }
    }

    private void FixedUpdate()
    {
        //grounded is false until proven otherwise 
        isGrounded = false;
        //if we not being slapped by a racket fall; if we're being hit, freeze (the computations...)!
        velocity = !isMoving ? velocity /*are we bouncing?*/ + gravityModifier * Physics.gravity : Vector3.zero;

        //if we're falling naturally, do some calculations to get us falling and colliding 
        if (velocity.magnitude > 0.001f && !isMoving)
        {
            float distance = velocity.magnitude; //length of direction that we are trying to move 
            Physics.Raycast(rb.position, Vector3.down, out hitInfo, length);
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

            //if we're grounded, bounce (but not infinitely, once we slow down, let's roll)
            if (!isMoving && isGrounded && velocity.magnitude > minBounceVelocity)
            {
                //add back velocity so that ball can bounce -- and not smack on the ground 
                velocity = new Vector3(velocity.x, bounceHieghtMultiplier, bounceDistanceMultiplier * velocity.z);
            }

            //if we're not being moved by the tennis racket, then move according to normal gravity and physics bruh 
            rb.position += velocity.normalized /*sheer direction*/ * distance /*"magnitude"*/ * Time.fixedDeltaTime /*smooth*/;
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
