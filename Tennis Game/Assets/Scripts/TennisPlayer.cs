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
    [SerializeField] protected Card[] deck; //list of cards (deck) tennis player uses to play

    [Header("Stats")]
    [SerializeField] protected float hitForce;
    [SerializeField] protected float moveSpeed;

    [Header("Ball")]
    [SerializeField] Ball ballScript;

    [Header("Ball Hitting")]
    [SerializeField] protected Rigidbody ball_rb; 
    [SerializeField] protected Transform aimTarget;
    [SerializeField] protected Transform netPositionTop;
    [SerializeField] protected Vector3[] path; 
    [SerializeField] protected int capacity = 25; //how to much increments over bezier curve path 

    [Header("Movement")]
    [SerializeField] protected Vector3 targetDirection; 

    [Header("RigidBody")]
    [SerializeField] protected Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        isPlayer = gameObject.CompareTag("Player"); 
        rb = gameObject.GetComponent<Rigidbody>(); //rigidbody component

        deck = new Card[6]; 
        allCardsScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<AllCards>(); 

        ballScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
        ball_rb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>(); //ball boi bod


        netPositionTop = GameObject.FindGameObjectWithTag("Net Top").transform; //top position of the net

        path = new Vector3[capacity]; //set the capacity (samples) of path graph

        //TEST
        deck[0] = allCardsScript.normal_a;
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
    protected virtual void hitBall(Rigidbody ball_rb) { }
    
    //Function to move the ball, wil be called in child classes, as hitting the ball works the same way, just defined differently based on hitBall()
    protected IEnumerator MoveBall(Rigidbody ball_rb, Card card)
    {
        //Don't let ball fall
        ballScript.isMoving = true;
        //Store previous position so that we can make a velocity calculation once the coroutine is finished 
        Vector3 previous = Vector3.zero; 
        //moves ball along bezier curve across multiple frames 
        //move the ball
        //modified version of net top position so that it aligns with player at midpoint between A and C so it's not curving wierdly 
        float mid = (transform.position + aimTarget.position).x / 2;
        Vector3 point = netPositionTop.position;
        point = new Vector3(mid, point.y, point.z);
        //get points 
        path = card.path(transform.position, point, aimTarget.position, capacity, isPlayer);
        for (int i = 0; i <= capacity - 1; i++)
        {
            previous = ball_rb.position; //set previous transform
            //where the acutal movement comes from (kind of)
            ball_rb.position = Vector3.Lerp(transform.position, path[i], card.speedMultiplier); 
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

        float mid = (transform.position + aimTarget.position).x / 2;
        Vector3 point = netPositionTop.position;
        point = new Vector3(mid, point.y, point.z);

        if (deck != null)
        {
            path = deck[0].path(transform.position, point, aimTarget.position, capacity, isPlayer);

            for (int i = 0; i <= capacity - 2; i++)
            {
                Debug.DrawLine(path[i], path[i + 1/*we are adding one at the end, so we're stopping two early*/]);
            }
        }
    }
}
