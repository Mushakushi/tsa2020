using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisPlayer : MonoBehaviour
{
    //note: player in this script refers to the tennis player, not just the player-player
    [Header("Child Identification")]
    [SerializeField] protected bool isPlayer; 

    [Header("Stats")]
    [SerializeField] protected float hitForce;
    [SerializeField] protected float moveSpeed;

    [Header("Ball")]
    Ball ballScript;

    [Header("Aiming")]
    [SerializeField] protected GameObject ball; 
    public static bool isBallGrounded = false; //is the ball grounded?
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
        isPlayer = gameObject.CompareTag("Player"); 
        rb = gameObject.GetComponent<Rigidbody>(); //rigidbody component
        ballScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
        ball = GameObject.FindGameObjectWithTag("Ball"); //ball boi
        netPositionTop = GameObject.FindGameObjectWithTag("Net Top").transform; //top position of the net 
    }

    // Update is called once per frame, but not here ... yet ....
    void Update()
    {
    }

    private void FixedUpdate()
    {
        //clamp tennis players to bounds based on whothey are 
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.5f, 1.5f), transform.position.y,
            Mathf.Clamp(transform.position.z, isPlayer ? -2f : 0.1f,
            isPlayer ? -0.1f : 2f));
        //Compute direction   
        ComputeDirection();
        //If we're actually moving...
        if (targetDirection.magnitude > 0f)
            rb.velocity = targetDirection * moveSpeed * Time.fixedDeltaTime; 
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
            hitBall(ball); 
        }
    }

    //Child classes will define how the process of hitting the ball works 
    protected virtual void hitBall(GameObject ball) { }
    
    //Function to move the ball, wil be called in child classes, as hitting the ball works the same way, just defined differently based on hitBall()
    protected IEnumerator MoveBall(GameObject ball)
    {

        //Store previous position so that we can make a velocity calculation once the coroutine is finished 
        Vector3 previous = Vector3.zero; 
        //moves ball along bezier curve across multiple frames 
        //move the ball
        for (float i = 0f; i <= 1f; i += step)
        {
            previous = ball.transform.position; //set previous transform
            //modified version of net top position so that it aligns with player at midpoint between A and C so it's not curving wierdly 
            float mid = (transform.position + aimTarget.position).x / 2;
            Vector3 point = Vector3.zero;
            point = Vector3.MoveTowards(point, new Vector3(mid, point.y, point.z), 1);
            //where the acutal movement comes from (kind of)
            ballScript.targetDirection = GetPointInPath(transform.position, point, aimTarget.position, i + step); 
            yield return null;
        }

        //Wait Until the ball is grounded, and, then, bounce it 
        yield return new WaitUntil(() => ballScript.isGrounded);
        //add back velocity so that ball can bounce -- and not smack on the ground 
        //ball.velocity = (ball.position - previous).normalized + new Vector3(0,0.5f,0.1f) /*add some force*/ / 10 /* "world scale" is small*/ / Time.fixedDeltaTime;
    }
     

    private Vector3 GetPointInPath(Vector3 A /*player pos*/, Vector3 point, Vector3 C/*aim pos*/, float t)
    {
        
        //there are infinite solutions for crossing these three points at any value of t, k represents the "best" (more-natural) value of t be the solution 
        float k = CalculateBestMiddlePoint(A, netPositionTop.position, C);
        //Re-writing of bezier curve to solve for what B should be so that the bezier curve travels through all three points 
        Vector3 B = (point - (1 - k) * (1 - k) * A - k * k * C) / (2 * (1 - k) * k);
        //to prevent B from creating a bezier curve that extends past A or C
        if (isPlayer)
            B.z = Mathf.Clamp(B.z, A.z, C.z); //A -> Player -> C
        else
            B.z = Mathf.Clamp(B.z, C.z, A.z); //C <- Opponent <- A
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
        if (isPlayer)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red; 

        for (float i = 0f; i <= 1; i += step)
        {
            Gizmos.DrawLine(GetPointInPath(transform.position, netPositionTop.position, aimTarget.position, i), 
                GetPointInPath(transform.position, netPositionTop.position, aimTarget.position, i + step)); 
        }
        
    }
}
