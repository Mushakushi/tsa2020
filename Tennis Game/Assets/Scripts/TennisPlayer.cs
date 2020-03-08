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
    [SerializeField] protected Card[] deck = new Card[6]; //list of cards (deck) tennis player uses to play
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
    [SerializeField] protected bool isMoveBallRunning = false;

    [Header("Aim Visual")]
    [SerializeField] protected Vector3[] path;
    [SerializeField] protected LineRenderer lineRenderer; 

    [Header("Movement")]
    [SerializeField] public Vector3 targetDirection; 

    [Header("RigidBody")]
    [SerializeField] protected Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        isPlayer = gameObject.CompareTag("Player");
        rb = gameObject.GetComponent<Rigidbody>(); //rigidbody component

        //TEST
        allCardsScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<AllCards>();
        deck[0] = allCardsScript.normal_a;
        deck[1] = allCardsScript.jumpShot_a;
        deck[2] = allCardsScript.normal_a;
        deck[3] = allCardsScript.jumpShot_a;
        deck[4] = allCardsScript.normal_a;
        deck[5] = allCardsScript.jumpShot_a;

        //if is player, set up UI for deck 
        if (isPlayer)
            SetUpDeckUI(); 
        
        //get the ball 
        ballScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
        ball_rb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>(); //ball boi bod


        netPositionTop = GameObject.FindGameObjectWithTag("Net Top").transform; //top position of the net

        path = new Vector3[capacity]; //set the capacity (samples) of path graph

        lineRenderer = GameObject.Find(isPlayer ? "Player Aim Line" : "Opponent Aim Line").GetComponent<LineRenderer>();
        lineRenderer.positionCount = capacity;
    }

    //Set up the deck UI (only for player)
    protected virtual void SetUpDeckUI() { }
    //Update the deck UI (only for player)
    protected virtual void UpdateDeckUI(int targetIndex) { }

    // Update is called once per frame
    void Update()
    {
        //UPDATE PATH TO HIT
        //modified version of net top position so that it aligns with player at midpoint between A and C so it's not curving wierdly 
        float mid = (transform.position + aimTarget.position).x / 2;
        Vector3 point = netPositionTop.position;
        point = Vector3.MoveTowards(point, new Vector3(mid, point.y, point.z), 0.5f);
        //get points 
        path = deck[currentCardIndex].path(transform.position, point, aimTarget.position, capacity, isPlayer);

        //RENDER LINE
        lineRenderer.SetPositions(path);
        lineRenderer.startColor = deck[currentCardIndex].color; 
    }

    private void FixedUpdate()
    {
        //clamp tennis players to bounds based on whothey are 
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.5f, 1.5f), transform.position.y,
                                                        //P    //O
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
            //take a snapshot of the current path, so that the ball doesn't update it's flight path when being hit 
            Vector3[] pathSnapshot = path;

            //Attempt to hitBall(); 
            HitBall(ball_rb, pathSnapshot, deck[currentCardIndex].effect); 
        }
    }

    //Child classes will define how the process of hitting the ball works 
    protected virtual void HitBall(Rigidbody ball_rb, Vector3[] path, Effect effect) { }
    
    //Function to move the ball, wil be called in child classes, as hitting the ball works the same way, just defined differently based on hitBall()
    protected IEnumerator MoveBall(Rigidbody ball_rb, Vector3[] path, Effect effect)
    {
        //Don't run this script multiple times!
        isMoveBallRunning = true; 

        //Don't let ball fall
        ballScript.isMoving = true;

        //Reset the bounce count
        ballScript.bounces = 0; 

        //Do the effect of the ball as long as it is not null
        effect?.Invoke(rb, ball_rb);

        //Move the ball 
        for (int i = 0; i <= capacity - 1; i++)
        {
            //where the acutal movement comes from (kind of)
            ball_rb.position = Vector3.Lerp(transform.position, path[i], deck[currentCardIndex].speedMultiplier);
            yield return null;
        }

        //Cycle to the next card 
        CycleDeck(ref currentCardIndex);

        //tell the ball that it is no longer moving and give it velocity based on movement 
        ballScript.velocity = transform.position - path[(int)Mathf.Ceil(path.Length/2)];
        print("ball velocity == " + (ballScript.velocity)); 
        
        ballScript.isMoving = false;

        //this script is no longer running 
        isMoveBallRunning = false; 
    }

    //Cycle through the deck 
    public void CycleDeck(ref int currentIndex)
    {
        //CURRENT CARD
        print("Cycling deck");
        //if the card that was just used has a cooldown, reset it's timer 
        if (deck[currentIndex].coolDown > 0)
        {
            deck[currentIndex].waitTime = 0;
            Debug.LogFormat("Card has been deactivated {0}", deck[currentIndex].name);
        }
        
        //NEW CARD
        //move to the next card in the list and get target card based on index
        int targetIndex = currentIndex;
        Card target = deck[currentIndex]; 
        MoveToNextCard(ref targetIndex, ref target); 
        
        //If the card has cooldown and is inactive
        if (target.coolDown > 0 && target.waitTime != target.coolDown)
        {
            //while the current card's waitime is less than its cooldown (are we still waiting for card to recharge?
            while (target.waitTime < target.coolDown /*waiting for recharge*/ && target.coolDown > 0 /*has a cooldown in the first place*/)
            {
                //Increase wait time (REMEBER: when waitTime increases to equal cooldown time; when we have waited, we have fully cool-downed)
                target.waitTime++; 

                //move to the next card 
                MoveToNextCard(ref targetIndex, ref target);
                //cyle repeats to find a cooldowned card....
            }
        }
        //we now have a card that is cool-downed

        //Return that card 
        print("NEXT CARD: " + targetIndex); 
        currentIndex = targetIndex;
        //Update UI (if you're player, of course!)
        if (isPlayer)
            UpdateDeckUI(targetIndex); 
    }
    
    public void MoveToNextCard(ref int index, ref Card target)
    {
        //move the the next card 
        index++; 
        //if outside of bounds, loop back to beginning
        if (index > deck.Length - 1)
            index = 0;
        //target Card based on the set index
        target = deck[index];
        print("currently checking/ moving to: " + index); 
    }

}
