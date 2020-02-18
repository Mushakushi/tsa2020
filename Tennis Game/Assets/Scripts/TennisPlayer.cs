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
    [SerializeField] protected GameObject ball;
    [SerializeField] protected Rigidbody ball_rb;
    [SerializeField] public bool canMoveBall = true; //accessible by BallCollsionSender (self-explanatory)
    [SerializeField] protected Transform aimTarget;
    [SerializeField] protected Transform netPositionTop;
    [SerializeField] protected float step = 0.01f; //how to much to increment over bezier curve path 

    [Header("Movement")]
    [SerializeField] protected Vector3 targetDirection; 

    [Header("RigidBody")]
    [SerializeField] protected Rigidbody rb; 

    // Start is called before the first frame update
    void Start()
    { 
        rb = gameObject.GetComponent<Rigidbody>(); //rigidbody component
        ball = GameObject.FindGameObjectWithTag("Ball"); //ball tennis players are vying to hit 
        ball_rb = ball.GetComponent<Rigidbody>(); //rigidbody of said ball
        netPositionTop = GameObject.FindGameObjectWithTag("Net Top").transform; //top position of the net 
    }

    // Update is called once per frame, but not here ... yet ....
    void Update()
    {
    }

    private void FixedUpdate()
    {
        //Moves player
        Move(targetDirection); 
    }

    //ai and player move differently 
    protected virtual void Move(Vector3 targetDirection) { }

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
            hitBall(ball); 
        }
    }

    //Child classes will define how the process of hitting the ball works 
    protected virtual void hitBall(GameObject ball) { }
    
    //Function to move the ball, wil be called in child classes, as hitting the ball works the same way, just defined differently based on hitBall()
    protected IEnumerator MoveBall(GameObject ball)
    {
        //Reset velocity of the ball, as to not build up uneccesary amount during rallying the ball
        ball_rb.velocity = Vector3.zero; 

        //disable gravity 
        ball_rb.useGravity = false;

        //Store previous position so that we can make a velocity calculation once the coroutine is finished 
        Vector3 previous = Vector3.zero; 
        //moves ball along bezier curve across multiple frames (while it is outside of another collider) 
        while (canMoveBall)
        {
            //move the ball
            for (float i = 0f; i <= 1f; i += step)
            {
                previous = ball.transform.position; //set previous transform
                ball.transform.position = Vector3.MoveTowards(transform.position,
                    GetPointInPath(transform.position, netPositionTop.position, aimTarget.position, i + step), hitForce);
                yield return null;
            }
            
            //once we're done with the process don't go all anime and hit the ball a ton of times
            break; 
        }
        

        //enable gravity 
        ball_rb.useGravity = true;

        //add back velocity so that ball can bounce -- and not smack on the ground 
        yield return new WaitUntil(()=> canMoveBall); //lambda expression
        ball_rb.velocity = (ball.transform.position - previous).normalized / 10 /* "world scale" is small*/ / Time.deltaTime;
    }
     

    private Vector3 GetPointInPath(Vector3 playerPos, Vector3 netPosTop, Vector3 aimPos, float t)
    {
        //equation of bezier curve: B(t)=(1−t)2P0+2(1−t)tP1+t2P2 where 0<=t<=1 (P0 == point 0, P1 == point 1 ...)
        float r = 1f - t;
        return (r * r * playerPos) + (2f * r * t * netPosTop) + (t * t * aimPos); 
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
