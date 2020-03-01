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
    [SerializeField] protected int currentCardIndex; 

    [Header("Stats")]
    [SerializeField] protected float hitForce;
    [SerializeField] protected float moveSpeed;

    [Header("Ball")]
    [SerializeField] Ball ballScript;

    [Header("Ball Hitting")]
    [SerializeField] protected Rigidbody ball_rb; 
    [SerializeField] protected Transform aimTarget;
    [SerializeField] protected Transform netPositionTop;
    [SerializeField] protected int capacity = 25; //how to much increments over bezier curve path 

    [Header("Aim Visual")]
    [SerializeField] protected Vector3[] path;
    [SerializeField] protected LineRenderer lineRenderer; 

    [Header("Movement")]
    [SerializeField] public Vector3 targetDirection; 

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

        lineRenderer = GameObject.Find(isPlayer ? "Player Aim Line" : "Opponent Aim Line").GetComponent<LineRenderer>();
        lineRenderer.positionCount = capacity;

        //TEST
        deck[0] = allCardsScript.normal_a;
        deck[1] = allCardsScript.jumpShot_a;
        deck[2] = allCardsScript.normal_a;
        deck[3] = allCardsScript.jumpShot_a;
        deck[4] = allCardsScript.normal_a;
        deck[5] = allCardsScript.jumpShot_a;
    }

    // Update is called once per frame
    void Update()
    {
        //Show Line
        lineRenderer.SetPositions(path);
        lineRenderer.startColor = deck[currentCardIndex].color; 
    }

    private void FixedUpdate()
    {
        //clamp tennis players to bounds based on whothey are 
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.5f, 1.5f), transform.position.y,
            Mathf.Clamp(transform.position.z, isPlayer ? -4f : 0.1f,
            isPlayer ? -0.5f : 4f));
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
    protected IEnumerator MoveBall(Rigidbody ball_rb)
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
        point = Vector3.MoveTowards(point, new Vector3(mid, point.y, point.z), 0.1f);
        //get points 
        path = deck[currentCardIndex].path(transform.position, point, aimTarget.position, capacity, isPlayer);
        for (int i = 0; i <= capacity - 1; i++)
        {
            previous = ball_rb.position; //set previous transform
            //where the acutal movement comes from (kind of)
            ball_rb.position = Vector3.Lerp(transform.position, path[i], deck[currentCardIndex].speedMultiplier); 
            yield return null;
        }

        //Cycle to the next card 
        currentCardIndex = CycleDeck(currentCardIndex); 

        ballScript.isMoving = false;
        //add back velocity so that ball can bounce -- and not smack on the ground 
        Vector3 moveDelta = ball_rb.position - previous; 
        ballScript.velocity = new Vector3(moveDelta.x, ballScript.bounceHieghtMultiplier, ballScript.bounceDistanceMultiplier * (isPlayer ? 1 : -1) /*forward or backwards?*/);

        
    }

    //Cycle through the deck 
    public int CycleDeck(int currentIndex)
    {
        print("Cycling deck");
        //if the card that was just used has a cooldown, reset it's timer 
        if (deck[currentIndex].coolDown > 0)
            deck[currentIndex].waitTime = 0; 

        //target index of card / move to the next card in the list 
        int targetIndex = currentIndex + 1;
        //go back to start of deck if over the max 
        if (targetIndex > deck.Length - 1)
            targetIndex = 0;
        //target card based on target index
        Card target = deck[targetIndex]; 
        
        //If the card has cooldown
        if (target.coolDown > 0)
        {
            Debug.LogFormat("Card has been deactivated {0}", target); 

            //increase wait time if the card has been deativated 
            if (target.waitTime < target.coolDown)
                target.waitTime++;

            //while the current card's waitime is less than its cooldown (are we still waiting?)
            while (target.waitTime < target.coolDown)
            {
                //go to the next card 
                targetIndex++;
                if (targetIndex > deck.Length)
                    targetIndex = 0;
                //update current card 
                target = deck[targetIndex]; 
            }
        }
        //we now have a card that is cool-downed

        //Return that card 
        print(targetIndex); 
        return targetIndex; 
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
            path = deck[currentCardIndex].path(transform.position, point, aimTarget.position, capacity, isPlayer);

            for (int i = 0; i <= capacity - 2; i++)
            {
                Debug.DrawLine(path[i], path[i + 1/*we are adding one at the end, so we're stopping two early*/]);
            }
        }
    }
}
