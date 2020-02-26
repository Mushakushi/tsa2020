using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisPlayer : MonoBehaviour
{
    //note: player in this script refers to the tennis player, not just the player-player
    [Header("Child Identification")]
    [SerializeField] protected bool isPlayer;

    [Header("Deck")]
    [SerializeField] protected AllCards allCardsScript; 
    [SerializeField] protected List<ICard> deck = new List<ICard>(6); //list of cards (deck) tennis player uses to play

    [Header("Stats")]
    [SerializeField] protected float hitForce;
    [SerializeField] protected float moveSpeed;

    [Header("Ball")]
    Ball ballScript;

    [Header("Aiming")]
    [SerializeField] protected Rigidbody ball_rb; 
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
        ball_rb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>(); //ball boi bod
        netPositionTop = GameObject.FindGameObjectWithTag("Net Top").transform; //top position of the net

        //TEST
        deck.Add(allCardsScript.normal_a); 
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
            hitBall(ball_rb); 
        }
    }

    //Child classes will define how the process of hitting the ball works 
    protected virtual void hitBall(Rigidbody ball) { }
    
    //Function to move the ball, wil be called in child classes, as hitting the ball works the same way, just defined differently based on hitBall()
    protected IEnumerator MoveBall(Rigidbody ball_rb, ICard card)
    {
        //Don't let ball fall
        ballScript.isMoving = true; 
        //Store previous position so that we can make a velocity calculation once the coroutine is finished 
        Vector3 previous = Vector3.zero; 
        //moves ball along bezier curve across multiple frames 
        //move the ball
        for (float i = 0f; i <= 1f; i += step)
        {
            previous = ball_rb.position; //set previous transform
            //modified version of net top position so that it aligns with player at midpoint between A and C so it's not curving wierdly 
            float mid = (transform.position + aimTarget.position).x / 2;
            Vector3 point = netPositionTop.position;
            point = new Vector3(mid, point.y, point.z); 
            //where the acutal movement comes from (kind of)
            ball_rb.position = card.
            yield return null;
        }

        ballScript.isMoving = false; 
        //add back velocity so that ball can bounce -- and not smack on the ground 
        ballScript.velocity = new Vector3(ball_rb.position.x - previous.x, ballScript.bounceHieghtMultiplier, ballScript.bounceDistanceMultiplier);
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
