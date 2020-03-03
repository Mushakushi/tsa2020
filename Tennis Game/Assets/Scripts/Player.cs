using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Player : TennisPlayer
{

    [Header("Deck UI")]
    [SerializeField] private GameObject[] deckUI = new GameObject[6];

    //set up UI of current cards
    private void Start()
    {
        //Get the original cardTemplate object 
        GameObject cardTemplate = GameObject.Find("Card Template");
        //Go over every element in deck UI 
        for (int i = 0; i <= deckUI.Length - 1; i++)
        {
            //Instantiate a new card that represent its corresponding card in "deck"
            GameObject currentCard = Instantiate(cardTemplate);
            //Get all of child TMP_Text objects 
            TMP_Text[] currentCardData = cardTemplate.GetComponentsInChildren<TMP_Text>();
            //Set each of the child objects to the corresponding data 
            currentCardData[0].text = deck[i].name;
            currentCardData[1].text = deck[i].waitTime.ToString(); 
            //add the current object to deck UI
            deckUI[i] = currentCard; 
        }
    }

    //update the UI of current cards
    private void Update()
    {
        
    }

    //Compute the movement direction 
    protected override void ComputeDirection()
    {
        targetDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * moveSpeed; 
    }

    protected override void hitBall(Rigidbody ball_rb, Vector3[] path)
    {
        //if we succssesfully hit the ball 
        if (Input.GetMouseButton(0))
            StartCoroutine(MoveBall(ball_rb, path)); 
    }
}
