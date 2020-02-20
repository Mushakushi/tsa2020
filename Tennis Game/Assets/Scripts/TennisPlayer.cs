﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisPlayer : MonoBehaviour
{
    //note: player in this script refers to the tennis player, not just the player-player
    [Header("Stats")]
    [SerializeField] protected float hitForce;
    [SerializeField] protected float moveSpeed;

    [Header("Aiming")]
    [SerializeField] protected Rigidbody ball_rb;
    [SerializeField] public static bool isBallGrounded = false; //is the ball grounded?
    [SerializeField] protected Transform aimTarget;
    [SerializeField] protected Transform netPositionTop;
    [SerializeField] protected float step = 0.1f; //how to much to increment over bezier curve path 

    [Header("Movement")]
    [SerializeField] protected Vector3 targetDirection; 

    [Header("RigidBody")]
    [SerializeField] protected Rigidbody rb; 

    // Start is called before the first frame update
    void Start()
    { 
        rb = gameObject.GetComponent<Rigidbody>(); //rigidbody component
        ball_rb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>(); //rigidbody of the ball the tennis player are vying to hit 
        netPositionTop = GameObject.FindGameObjectWithTag("Net Top").transform; //top position of the net 
    }

    // Update is called once per frame, but not here ... yet ....
    void Update()
    {
    }

    private void FixedUpdate()
    {
        //clamp tennis players to bounds based on whothey are 
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1, 1), transform.position.y,
            Mathf.Clamp(transform.position.z, gameObject.CompareTag("Opponent") ? 0f : -2f,
            gameObject.CompareTag("Opponent") ? 2f : 0f));
        //Compute direction   
        ComputeDirection();
        //If we're actually moving...
        if (targetDirection.magnitude > 0f)
            rb.velocity = targetDirection * moveSpeed * Time.deltaTime; 
    }
    
    //player and AI move differently 
    protected virtual void ComputeDirection(){}

    //return the ball. This is put inside of OnTriggerStay becuase each of these classes will use it anyways!
    private void OnTriggerStay(Collider other)
    {
        //is this the ball, which we are hitting? 
        if (other.CompareTag("Ball"))
        {
            //Attempt to hitBall(); 
            hitBall(ball_rb); 
        }
    }

    //Child classes will define how the process of hitting the ball works 
    protected virtual void hitBall(Rigidbody ball) { }
    
    //Function to move the ball, wil be called in child classes, as hitting the ball works the same way, just defined differently based on hitBall()
    protected IEnumerator MoveBall(Rigidbody ball)
    {
        //Reset velocity of the ball, as to not build up uneccesary amount during rallying the ball
        ball.velocity = Vector3.zero; 

        //disable gravity 
        ball.useGravity = false;

        //Store previous position so that we can make a velocity calculation once the coroutine is finished 
        Vector3 previous = Vector3.zero; 
        //moves ball along bezier curve across multiple frames 
        //move the ball
        for (float i = 0f; i <= 1f; i += step)
        {
            previous = ball.position; //set previous transform
            ball.position = Vector3.MoveTowards(ball.position, //ith point in GetPointInPath below 
                GetPointInPath(transform.position, netPositionTop.position, aimTarget.position, i + step /*move to the next step*/),
                hitForce);//move as fast as hitForce dictates
            yield return null;
        }

        //enable gravity 
        ball.useGravity = true;

        //Wait Until the ball is grounded, and, then, bounce it 
        yield return new WaitUntil(() => isBallGrounded);
        //add back velocity so that ball can bounce -- and not smack on the ground SS
        Vector3 diff = (ball.position - previous).normalized; 
        ball.velocity = new Vector3(0,-1 ,1) * diff.x / 5 /* "world scale" is small*/ / Time.deltaTime;
    }
     

    private Vector3 GetPointInPath(Vector3 A /*player pos*/, Vector3 point, Vector3 C/*aim pos*/, float t)
    {
        //move point (point that we are trying to go through) to tennis player's position so that it's no curving wierdly all of the time 
        point = Vector3.MoveTowards(point, new Vector3(A.x, point.y, point.z), 1);
        //there are infinite solutions for crossing these three points at any value of t, k represents the "best" (more-natural) value of t be the solution 
        float k = CalculateBestMiddlePoint(A, netPositionTop.position, C);
        //Re-writing of bezier curve to solve for what B should be so that the bezier curve travels through all three points 
        Vector3 B = (point - (1 - k) * (1 - k) * A - k * k * C) / (2 * (1 - k) * k); 
        //to prevent B from creating a bezier curve that extends past A or C
        if (GameObject.CompareTag("Opponent"))
            B.z = Mathf.Clamp(B, C.z, A.z); 
        else 
            B.z = Mathf.Clamp(B, A.z, C.z); 
        //equation of bezier curve: B(t)=(1−t)2P0+2(1−t)tP1+t2P2 where 0<=t<=1 (P0 == point 0, P1 == point 1 ...)
        return (1f - t) * (1f - t) * A + 2f * (1f - t) * t * B + t * t * C; 
    }

    private float CalculateBestMiddlePoint(Vector3 A, Vector3 point /*point that we are trying to go through, aka netTopPos*/, Vector3 C)
    {
        //There is probably a better method, but I run into the problem where the bezier curve goes backwards, so it remain at safe number for now....
        float a = Vector2.Distance(point, A);
        float b = Vector2.Distance(point, C); 
        return a / (a + b);

        //thanks to @Bunny83 and @SparrowsNest for all their help! 
        //https://answers.unity.com/questions/1700903/how-to-make-quadratic-bezier-curve-through-3-point.html?_ga=2.167879905.1985838484.1581976785-1145140738.1537626484
    }

    private void OnDrawGizmos()
    {
        //distinguishes from player and ai aim
        if (gameObject.CompareTag("Opponent"))
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue; 

        for (float i = 0f; i <= 1; i += step)
        {
            Gizmos.DrawLine(GetPointInPath(transform.position, netPositionTop.position, aimTarget.position, i), 
                GetPointInPath(transform.position, netPositionTop.position, aimTarget.position, i + step)); 
        }
        
    }
}
