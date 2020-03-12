using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : TennisPlayer
{

    [Header("Deck UI")]
    [SerializeField] private GameObject[] deckUI;
    [SerializeField] private GameObject cardTemplate; //Get the original cardTemplate object 
    [SerializeField] private float moveStep;

    [Header("Spacing")]
    [SerializeField] private Transform start;
    [SerializeField] private float spacing;

    [Header("Animation")]
    [SerializeField] AnimationCurve easeCurve; 

    #region PLAYER-DECK UI
    //set up UI of current cards
    protected override void SetUpDeckUI()
    {
        //Get the UI Canvas 
        Canvas canvas = GameObject.Find("UI").GetComponent<Canvas>();
        //Get the staring position of the cards 
        start = GameObject.Find("Card UI Start").GetComponent<Transform>();
        //Go over every element in deck UI 
        for (int i = 0; i <= activeDeck.Length - 1; i++)
        {
            //Instantiate a new card that represent its corresponding card in "deck"
            GameObject currentCard = Instantiate(cardTemplate, canvas.transform);
            //Get Image that holds card data 
            Image card = currentCard.transform.GetChild(0).gameObject.GetComponent<Image>();
            //offset the card a little
            LeanTween.move(card.gameObject, start.position + (i == 0 ? Vector3.zero : Vector3.right * spacing), moveStep).setEase(easeCurve); 
            TMP_Text[] currentCardData = card.GetComponentsInChildren<TMP_Text>();
            //Set each of the child objects to the corresponding data
            print(i); 
            print(currentCardData[0].text);
            print(activeDeck[i].name); 
            currentCardData[0].text = activeDeck[i].name; //set up name ONLY HERE (why do it again?)
            UpdateCardData(ref currentCardData, activeDeck[i].name, activeDeck[i].waitTime); 
            //add the current object to deck UI
            deckUI[i] = currentCard; 
        }
    }

    //update the UI of current cards
    protected override void UpdateDeckUI(int targetIndex)
    {
        //go through every element of the deck 
        for (int i = 0; i < activeDeck.Length; i++)
        {
            //EXAMPLE: 
            //{0}, {1}, {2}, {3}
            //target index == 1
            //move {1} first (TI + i == 1), {2} second (TI + i == 2), {3} third (TI + i == 3), 
            //(TI + i == 4) OUT OF BOUNDS, so....
            //{0} fourth (TI + i/*==4*/ - length/*4*/ = 0) 

            int index = targetIndex + i;
            //if too big, loop back around 
            if (index > activeDeck.Length - 1)
                index -= activeDeck.Length;

            //DEBUGG
            print("moving card at index: " + index);

            //GET CARD AND ITS DATA
            //Get the card (<Image>), yo 
            Image card = deckUI[index].transform.GetChild(0).GetComponent<Image>(); 
            //Get the data (<TMP_Text>), or text, from the child object (fields e.g. name, waitTime, etc.)
            TMP_Text[] currentCardData = card.GetComponentsInChildren<TMP_Text>();
            
            //UPDATE CARD
            UpdateCardData(ref currentCardData, activeDeck[i].name, activeDeck[i].waitTime); 
            
            //MOVE CARD
            //get anchoredPosition of Image componenet
            Vector2 cardTransform = card.rectTransform.anchoredPosition;
            //Move deck around 
            LeanTween.move(card.gameObject, start.position + (i == targetIndex ? Vector3.zero : Vector3.right * spacing), moveStep);
            //fun fact: I was doing movement with an IEnumerator and Action<Vector2> stuff; it was crazy. LeanTween to the rescue!!!
        }
    }
    
    //Updates the text on the cards 
    private void UpdateCardData(ref TMP_Text[] data, string name, int waitTime)
    {
        //update it! (it's almost like setting up a constructor! (we could make it like that, but why?))
        data[0].text = name;
        data[1].text = waitTime.ToString(); 
    }
    #endregion

    //Compute the movement direction 
    protected override void ComputeDirection()
    {
        targetDirection = canMove ? new Vector3(Input.GetAxis("Horizontal"), targetDirection.y, Input.GetAxis("Vertical")) * moveSpeed :
            Vector3.zero; 
    }

    protected override void HitBall(Rigidbody ball_rb, Vector3[] path, Effect effect)
    {
        //if we succssesfully hit the ball 
        if (Input.GetMouseButton(0) && !isMoveBallRunning && canMove)
            StartCoroutine(MoveBall(ball_rb, path, effect)); 
    }
}
