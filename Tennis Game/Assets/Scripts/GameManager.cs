using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    [Header("Serving?")]
    [SerializeField] public bool isServing = false;

    [Header("Score")]
    [SerializeField] public int playerScore;
    [SerializeField] public int opponentScore;

    [Header("Tennis Balll")]
    [SerializeField] private Ball ballScript;

    [Header("Tennis Players")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject opponent;

    [Header("Tennis Court")]
    [SerializeField] private Transform playerStart;
    [SerializeField] private Transform opponentStart;

    [Header("Tennis Rules")]
    public static GameManager instance;//instance (singleton) of this class
    public event Action onSetEnd; //what happens when the set ends?
    public event Action onSetStart; //what happens when the set starts?

    [Header("UI")]
    [SerializeField] private Image scoreBoard;
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text opponentScoreText;

    [Header("Ease Types")]
    [SerializeField] private AnimationCurve scoreCurve;

    //Set up singleton
    private void Awake()
    {
        instance = this; //singleton pattern
        instance.onSetStart += OnSetStart; 
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        opponent = GameObject.FindGameObjectWithTag("Opponent");

        playerStart = GameObject.Find("Player Start").GetComponent<Transform>();
        opponentStart = GameObject.Find("Opponent Start").GetComponent<Transform>(); 

        scoreBoard = GameObject.Find("Score").GetComponent<Image>();
        playerScoreText = scoreBoard.transform.GetChild(0).GetComponent<TMP_Text>();
        opponentScoreText = scoreBoard.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //Scores point if opponet sends it out of bounds in one hit 
        //Scores point if bounces > 2 on other side 

        //Opponent scores a point!
        if ((ballScript.rb.position.z > 4f && ballScript.bounces == 0) || (ballScript.bounces > 1 && ballScript.rb.position.z < 0.5f))
            ScorePoint(false);
        //Player scores a point!
        else if ((ballScript.rb.position.z < -4f && ballScript.bounces == 0) || (ballScript.bounces > 1 && ballScript.rb.position.z > 0.1f))
            ScorePoint(true);

        //Check if ball's position is less than 0 (that's not supposed to happen, duh)
        if (ballScript.rb.position.y < 0f)
            Debug.LogError("Ball REALLY went out of bounds!"); 
    }

    void ScorePoint(bool playerScored)
    {
        //stop moving, boru!
        ballScript.isMoving = true;

        //Move player and opponent to starting positions 
        LeanTween.move(player, playerStart.position, 2f);
        LeanTween.move(opponent, opponentStart.position, 1f);

        //move to winner
        if (playerScored)
        {
            UpdateScore(playerScoreText, ref playerScore); print("player scores!");
        }
        else
        {
            UpdateScore(opponentScoreText, ref opponentScore);
        }

        onSetEnd();

        //Wait until player is halfway between its starting point (the set DOES NOT BEGIN until players have returned to starting positions)
        StartCoroutine(WaitUntil(
            //don't check the y, becuase gravity doesn't play nice
            () => Mathf.RoundToInt(player.transform.position.x) == Mathf.RoundToInt(playerStart.position.x) 
            && Mathf.RoundToInt(player.transform.position.z) == Mathf.RoundToInt(playerStart.position.z),
            () => {
                ballScript.rb.position = (playerScored ? player.transform.position : opponent.transform.position) + Vector3.up * 0.5f; /*give ball  to winner*/
                StartCoroutine(WaitFor(1f, onSetStart));
            })
        ); 
    }

    //Two functions that do the same thing, just differently (waiting, then doing! [or just waiting, if you throw in null])
    IEnumerator WaitFor(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke(); 
    }

    IEnumerator WaitUntil(Func<bool> predicate, Action action)
    {
        yield return new WaitUntil(predicate); 
        action?.Invoke();
    }

    void UpdateScore(TMP_Text text, ref int score)
    {
        score++;
        string value = score.ToString();
        text.text = value;
        LeanTween.scale(text.gameObject, Vector3.one * 5f, 0.1f).setEase(scoreCurve); 
    }

    //Extra functionality
    void OnSetStart()
    {
        //Swap the x positions of the player start positions (a rule in tennis) for next set 
        float temp = playerStart.position.x;
        playerStart.position = new Vector3(opponentStart.position.x, playerStart.position.y, playerStart.position.z);
        opponentStart.position = new Vector3(temp, opponentStart.position.y, opponentStart.position.z);
        //give the ball to the winner 
    }

    void OnSetEnd()
    {
        
    }
}
