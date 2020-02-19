using System.Collections;
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
        if (targetDirection.
    }
    
    //player and AI move differently 
    protected virtual void Move(Vector3 targetDirection)
    {
        
    }

    //Clamp positions to respective halves 
    protected void ClampPosition()
    {
        //clamp tennis players to bounds based on whothey are 
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1, 1), transform.position.y,
            Mathf.Clamp(transform.position.z, gameObject.CompareTag("Opponent") ? 0f : -2f,
            gameObject.CompareTag("Opponent") ? 2f : 0f));
    }

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
                GetPointInPath(transform.position, netPositionTop.position, aimTarget.position, i + step/*move to the next step*/),
                hitForce);//move as fast as hitForce dictates
            yield return null;
        }

        Debug.LogError("MoveBall finished"); 

        //enable gravity 
        ball.useGravity = true;

        //add back velocity so that ball can bounce -- and not smack on the ground SS
        ball.velocity = (ball.transform.position - previous) / 10 /* "world scale" is small*/ / Time.deltaTime;
    }
     

    private Vector3 GetPointInPath(Vector3 A /*player pos*/, Vector3 point, Vector3 C/*aim pos*/, float t)
    {
        //there are infinite solutions for crossing these three points at any value of t, k represents the "best" (more-natural) value of t be the solution 
        float k = CalculateBestMiddlePoint(A, netPositionTop.position, C);
        //Re-writing of bezier curve to solve for what B should be so that the bezier curve travels through all three points 
        Vector3 B = (point - (1 - k) * (1 - k) * A - k * k * C) / (2 * (1 - k) * k); 
        //equation of bezier curve: B(t)=(1−t)2P0+2(1−t)tP1+t2P2 where 0<=t<=1 (P0 == point 0, P1 == point 1 ...)
        return (1f - t) * (1f - t) * A + 2f * (1f - t) * t * B + t * t * C; 
    }

    private float CalculateBestMiddlePoint(Vector3 A, Vector3 point /*point that we are trying to go through, aka netTopPos*/, Vector3 C)
    {
        float a = Vector2.Distance(point, A);
        float b = Vector2.Distance(point, C); 
        return a / (a + b);

        //thanks to @Bunny83 for all his help! 
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
