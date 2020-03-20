using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisPlayer : MonoBehaviour
{
    //note: player in this script refers to the tennis player, not just the player-player
    [Header("Child Identification")]
    [SerializeField] protected bool isPlayer;

    [Header("Deck")]
    [SerializeField] protected Card[] activeDeck; //active deck; 
    [SerializeField] protected int currentCardIndex;
    [SerializeField] protected Card[] setDeck = new Card[6]; //list of cards (deck) tennis player uses to play
    [SerializeField] protected Card[] serveDeck = new Card[3]; //list of cards tennis player uses to serve

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
    [SerializeField] protected bool canMove = true; //can we move?

    [Header("RigidBody")]
    [SerializeField] protected Rigidbody rb;

    private void Awake()
    {
        //ADD FUNCTIONS TO GAME MANAGER EVENTS
        MatchManager.instance.gameEndEvent += OnGameEnd;
        MatchManager.instance.gameStartEvent += OnGameStart;
        MatchManager.instance.serveEvent += OnServing; 
    }

    #region Start
    // Start is called before the first frame update
    private void Start()
    {
        isPlayer = gameObject.CompareTag("Player"); //identify whether this is the player or not
        rb = gameObject.GetComponent<Rigidbody>(); //rigidbody component

        //TEST]
        setDeck[0] = AllCards.normal_a;
        setDeck[1] = AllCards.jumpShot_a;
        setDeck[2] = AllCards.normal_a;
        setDeck[3] = AllCards.jumpShot_a;
        setDeck[4] = AllCards.normal_a;
        setDeck[5] = AllCards.jumpShot_a;

        serveDeck[0] = AllCards.normal_s;
        serveDeck[1] = AllCards.normal_s;
        serveDeck[2] = AllCards.normal_s;

        activeDeck = serveDeck; 
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
    #endregion

    #region Events
    //what to do before the game starts 
    private void OnPreGame()
    {

    }

    //What to do when the set ends 
    private void OnGameEnd()
    {
        canMove = false;
        activeDeck = serveDeck;
        if (isPlayer) SetUpDeckUI(); 
    }

    //what to do when serving 
    private void OnServing()
    {
        //if we are the player and are supposed to serve 
        if (isPlayer && ScoreManager.IsPlayerServing())
        {
            activeDeck = setDeck;
            SetUpDeckUI(); //set up the UI
            //wait until the player tosses the ball
            Utility.WaitUntil(() => Input.GetMouseButton(0), TossBall); 
        }
        //if we are the opponenet (if the player isn't serving, then we are)
        else if (!isPlayer)
        {
            activeDeck = setDeck;
            //opponent tosses the ball immediately 
            TossBall(); 
        }
        print(activeDeck);
    }

    //What to do when the set starts
    private void OnGameStart()
    {
        canMove = true;
    }

    private void OnDestroy()
    {
        MatchManager.instance.gameEndEvent -= OnGameEnd;
        MatchManager.instance.gameStartEvent -= OnGameStart;
        MatchManager.instance.serveEvent -= OnServing; 
    }
    #endregion

    #region Player UI Virtual Functions
    //Set up the deck UI (only for player)
    protected virtual void SetUpDeckUI() { }
    //Update the deck UI (only for player)
    protected virtual void UpdateDeckUI(int targetIndex) { }
    #endregion 

    // Update is called once per frame
    void Update()
    {
        //UPDATE PATH TO HIT
        //modified version of net top position so that it aligns with player at midpoint between A and C so it's not curving wierdly 
        float mid = (transform.position + aimTarget.position).x / 2;
        Vector3 point = netPositionTop.position;
        point = Vector3.MoveTowards(point, new Vector3(mid, point.y, point.z), 0.5f);
        //get points 
        path = activeDeck[currentCardIndex].path(transform.position, point, aimTarget.position, capacity, isPlayer);

        //RENDER LINE
        lineRenderer.SetPositions(path);
        lineRenderer.startColor = activeDeck[currentCardIndex].color; 
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

    #region Ball Hitting Functionality
    //return the ball. This is put inside of OnTriggerStay becuase each of these classes will use it anyways!
    private void OnTriggerStay(Collider other)
    {
        //is this the ball, which we are hitting? 
        if (other.CompareTag("Ball"))
        {
            //take a snapshot of the current path, so that the ball doesn't update it's flight path when being hit 
            Vector3[] pathSnapshot = path;

            //Attempt to hitBall(); 
            HitBall(ball_rb, pathSnapshot, activeDeck[currentCardIndex].effect); 
        }
    }

    //toss the ball in the air for serving
    private void TossBall()
    {
        //disable the racket 
        transform.GetChild(0).gameObject.isStatic = true; 
        ballScript.AddForce(Vector3.up * 2f);
        //enable the racket so that we can hit the ball
        Utility.WaitUntil(() => ballScript.GetVelocity().y < 0f, () => transform.GetChild(0).gameObject.isStatic = true);
    }

    //Child classes will define how the process of hitting the ball works 
    protected virtual void HitBall(Rigidbody ball_rb, Vector3[] path, Effect effect) { }
    
    //Function to move the ball, wil be called in child classes, as hitting the ball works the same way, just defined differently based on hitBall()
    protected IEnumerator MoveBall(Rigidbody ball_rb, Vector3[] path, Effect effect)
    {
        //Don't run this script multiple times!
        isMoveBallRunning = true; 

        //Don't let ball fall
        ballScript.canMove = false;

        //Reset the bounce count
        ballScript.bounces = 0; 

        //Do the effect of the ball as long as it is not null
        effect?.Invoke(rb, ball_rb);

        //Move the ball 
        for (int i = 0; i <= capacity - 1; i++)
        {
            //where the acutal movement comes from (kind of)
            ball_rb.position = Vector3.Lerp(transform.position, path[i], activeDeck[currentCardIndex].speedMultiplier);
            yield return null;
        }

        //Cycle to the next card 
        CycleDeck(ref currentCardIndex);

        //tell the ball that it is no longer moving and give it velocity based on movement 
        ballScript.SetVelocity(!isPlayer ? transform.position - path[(int)Mathf.Ceil(path.Length/2)] : path[(int)Mathf.Ceil(path.Length/2)] - transform.position);
        print("ball velocity == " + (ballScript.GetVelocity())); 
        
        ballScript.canMove = true;

        //this script is no longer running 
        isMoveBallRunning = false; 
    }
    #endregion

    #region Deck Functionality
    //Cycle through the deck 
    public void CycleDeck(ref int currentIndex)
    {
        //CURRENT CARD
        print("Cycling deck");
        //if the card that was just used has a cooldown, reset it's timer 
        if (activeDeck[currentIndex].coolDown > 0)
        {
            activeDeck[currentIndex].waitTime = 0;
            Debug.LogFormat("Card has been deactivated {0}", activeDeck[currentIndex].name);
        }
        
        //NEW CARD
        //move to the next card in the list and get target card based on index
        int targetIndex = currentIndex;
        Card target = activeDeck[currentIndex]; 
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
        if (index > activeDeck.Length - 1)
            index = 0;
        //target Card based on the set index
        target = activeDeck[index];
    }
    #endregion

}
