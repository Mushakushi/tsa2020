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

    [Header("UI")]
    [SerializeField] private Image scoreBoard;
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text opponentScoreText; 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        opponent = GameObject.FindGameObjectWithTag("Opponent");

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
        if ((ballScript.rb.position.z > 4f && ballScript.bounces == 0) || (ballScript.bounces > 2 && ballScript.rb.position.z < 0.5f))
            ScorePoint(false);
        //Player scores a point!
        else if ((ballScript.rb.position.z < -4f && ballScript.bounces == 0) || (ballScript.bounces > 2 && ballScript.rb.position.z > 0.1f))
            ScorePoint(true);

        //Check if ball's position is less than 0 (that's not supposed to happen, duh)
        if (ballScript.rb.position.y < 0f)
            Debug.LogError("Ball REALLY went out of bounds!"); 
    }

    void ScorePoint(bool playerScored)
    {
        //stop moving, boru!
        ballScript.isMoving = true;

        //move to winner
        if (playerScored)
        {
            playerScore++;
            ballScript.rb.position = player.transform.position;
            playerScoreText.text = playerScore.ToString(); 
        }
        else
        {
            opponentScore++;
            ballScript.rb.position = opponent.transform.position;
            opponentScoreText.text = opponentScore.ToString(); 
        }

        //Hold the ball!
        //TODO: send to a set point!
        ballScript.velocity = Vector3.zero;
        player.transform.position = Vector3.back * 2;
        opponent.transform.position = Vector3.forward * 2;

        StartCoroutine(Wait()); 
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f); 
    }
}
