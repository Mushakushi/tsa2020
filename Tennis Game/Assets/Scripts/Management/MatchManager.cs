using System; 
using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class MatchManager : MonoBehaviour
{
    [Header("Serving?")]
    public bool isServing = false;

    [Header("Tennis Balll")]
    [SerializeField] private Ball ballScript;

    [Header("Tennis Players")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject opponent;

    [Header("Tennis Court")]
    [SerializeField] private Transform playerStart;
    [SerializeField] private Transform opponentStart;
    [SerializeField] private Transform serveArea; 

    [Header("Tennis Rules")]
    public static MatchManager instance;//instance (singleton) of this class
    public event Action gameStartEvent; //what happens when the set starts?
    public event Action serveEvent; //what happens the the winner serves?
    public event Action gameEndEvent; //what happens when the set ends?
    public event Action setEndEvent; //what happens when the set start?
    public event Action setStartEvent; //what happens when the set starts?

    [Header("UI")]
    [SerializeField] private Image scoreBoard;
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text opponentScoreText;

    [Header("Ease Types")]
    private const float _90inEulers = 0.7071068f; //90 degrees in eulers
    [SerializeField] private AnimationCurve scoreCurve;

    //Set up singleton
    private void Awake()
    {
        //singleton pattern (first if statements is just to make sure duplicates don't happen)
        if (instance != null && instance == this)
        {
            Destroy(gameObject); 
        }
        else
        {
            instance = this;
        }

        //Set up some events
        instance.gameStartEvent += OnGameStart;
        instance.gameEndEvent += OnGameEnd; 
    }

    // Start is called before the first frame update
    void Start()
    {
        ballScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>(); 

        player = GameObject.FindGameObjectWithTag("Player");
        opponent = GameObject.FindGameObjectWithTag("Opponent");

        playerStart = GameObject.Find("Player Start").GetComponent<Transform>();
        opponentStart = GameObject.Find("Opponent Start").GetComponent<Transform>();
        serveArea = GameObject.Find("Serve Area").GetComponent<Transform>(); 

        scoreBoard = GameObject.Find("Score").GetComponent<Image>();
        playerScoreText = scoreBoard.transform.GetChild(0).GetComponent<TMP_Text>();
        opponentScoreText = scoreBoard.transform.GetChild(1).GetComponent<TMP_Text>();

        //start off the match 
        //Debug.LogFormat("Player is {0}serving first", ScoreManager.IsPlayerServing() ? string.Empty : "not "); 
        PreMatchEvent(); 
    }

    // Update is called once per frame
    void Update()
    {
        //Tennis rules --> 40pts to win a game --> 6 games to win a set --> 2 sets to win a match

        //Scores point if opponet sends it out of bounds in one hit 
        //Scores point if bounces > 2 on other side 

        //Opponent scores a point!
        if ((ballScript.rb.position.z > 4f && ballScript.bounces == 0) || (ballScript.bounces > 1 && ballScript.rb.position.z < 0.5f))
            ScorePoint(false);
        //Player scores a point!
        else if ((ballScript.rb.position.z < -4f && ballScript.bounces == 0) || (ballScript.bounces > 1 && ballScript.rb.position.z > 0.1f))
            ScorePoint(true);

        //Check if ball's position is less than 0 (that's not supposed to happen, duh)
        if (ballScript.rb.position.y < -5f)
            Debug.LogError("Ball REALLY went out of bounds!");
    }

    void ScorePoint(bool playerScored)
    {
        //stop moving, boru!
        ballScript.canMove = false;

        //Move player and opponent to starting positions 
        LeanTween.move(player, playerStart.position, 2f);
        LeanTween.move(opponent, opponentStart.position, 1f);

        //Score point 
        if (playerScored)
            ScoreManager.IncreaseScore(true);
        else
            ScoreManager.IncreaseScore(false);

        //if the set is won, do stuff relating to ending/starting a new set, if not, just continue on with the set; next game!
        if (ScoreManager.IsSetWon())
            setEndEvent(); 
        else
            gameEndEvent();

        //Wait until player is halfway between its starting point (the set DOES NOT BEGIN until players have returned to starting positions)
        StartCoroutine(Utility.WaitUntil(
            //don't check the y, becuase gravity doesn't play nice
            () => Mathf.RoundToInt(player.transform.position.x) == Mathf.RoundToInt(playerStart.position.x) 
            && Mathf.RoundToInt(player.transform.position.z) == Mathf.RoundToInt(playerStart.position.z),
            () => {
                AwardBall(); 
                //wait a bit, then start the serve event
                StartCoroutine(Utility.WaitFor(1f, serveEvent));
            })
        ); 
    }

    void AwardBall()
    {
        //alternate serving of sets (give ball to tennis player based on set number
        ballScript.rb.position = (ScoreManager.IsPlayerServing() ? player.transform.position : opponent.transform.position);
    }
    

    void UpdatePointText(TMP_Text text, string value)
    {
        //set score text to second column (point names) and level
        LeanTween.rotateAround(text.gameObject, Vector3.up, 360, 0.2f); 
        LeanTween.scale(text.gameObject, Vector3.one * 2f, 0.5f).setEase(scoreCurve).setLoopPingPong(1);
        Utility.WaitUntil(() => text.rectTransform.rotation.eulerAngles.y >= 90f, () => { text.text = value; });
    }

    #region Events
    void PreMatchEvent()
    {
        //Move To Starting Positions 
        player.transform.position = playerStart.position;
        opponent.transform.position = opponentStart.position; 
        Utility.WaitFor(1f);
        //Start the game
        AwardBall();
        gameStartEvent();
        serveEvent();
    }

    void OnGameEnd()
    {
        //Update the player and opponent scores
        UpdatePointText(playerScoreText, ScoreManager.playerPointIndex.FormatPointAsString());
        UpdatePointText(opponentScoreText, ScoreManager.opponentPointIndex.FormatPointAsString());
        Debug.LogFormat("player: {0} \n opponent: {1}", ScoreManager.playerPointIndex.FormatPointAsString(),
            ScoreManager.opponentPointIndex.FormatPointAsString()); 
    }

    void OnGameStart()
    {
        //Swap the x positions of the player start positions (a rule in tennis) for next set 
        playerStart.position.FlipX(); 
        opponentStart.position.FlipX();
        //Swap serve area 
        serveArea.position.FlipX(); 
    }

    void OnSetEnd()
    {
        
    }
    #endregion
}
