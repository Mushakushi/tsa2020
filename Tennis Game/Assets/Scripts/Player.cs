using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : TennisPlayer
{

    [Header("Deck UI")]
    [SerializeField] private GameObject[] deckUI = new GameObject[6];
    [SerializeField] private GameObject cardTemplate; //Get the original cardTemplate object 
    [SerializeField] private float moveStep; 

    //set up UI of current cards
    protected override void SetUpDeckUI()
    {
        //Get the UI Canvas 
        Canvas canvas = GameObject.Find("UI").GetComponent<Canvas>(); 
        //Go over every element in deck UI 
        for (int i = 0; i <= deckUI.Length - 1; i++)
        {
            //Instantiate a new card that represent its corresponding card in "deck"
            GameObject currentCard = Instantiate(cardTemplate, canvas.transform);
            //Get Image that holds card data 
            Image card = currentCard.transform.GetChild(0).gameObject();
            //offset the card a little
            card.rectTransform.anchoredPosition = new Vector2((i * 100) - 150, 0); 
            TMP_Text[] currentCardData = card.GetComponentsInChildren<TMP_Text>();
            //Set each of the child objects to the corresponding data
            UpdateCardData(currentCardData, deck[i].waitTime); 
            //add the current object to deck UI
            deckUI[i] = currentCard; 
        }
    }

    //update the UI of current cards
    protected override void UpdateDeckUI(int targetIndex)
    {
        //go through every element of the deck 
        for (int i = 0; i < deck.Length - 1; i++)
        {
            //EXAMPLE: 
            //{0}, {1}, {2}, {3}
            //target index == 2
            //move {2} first (TI + i == 2), {3} second (TI + i == 3), 
            //{0} third (TI + i /*==4*/ - length /*==4*/ == 0), {1} fourth (TI + i/*==5*/ - length/*4*/ = 1) 

            int index = targetIndex + i;
            //if too big, loop back around 
            if (index > deck.Length - 1)
            {
                index -= deck.Length;
            }
            
            //GET CARD AND ITS DATA
            //Get the card (<Image>), yo 
            Image card = deckUI[index].transform.GetChild(0); 
            //Get the data (<TMP_Text>), or text, from the child object (fields e.g. name, waitTime, etc.)
            TMP_Text[] currentCardData = card.GetComponentsInChildren<TMP_Text>();
            
            //UPDATE CARD
            UpdateCardData(currentCardData, deck[i].name, deck[i].waitTime); 
            
            //MOVE CARD
            //get anchoredPosition of Image componenet
            Vector2 cardTransform = card.rectTransform.anchoredPosition;  
            //Move deck around 
            StartCoroutine(MoveCard(ref cardTransform, targetIndex));

        }
    }
    
    //Cards do not have an Update loop to lerp continuously, thus the use of a couroutine
    private IEnumerator MoveCardUI(ref Vector2 card, int i)
    {
        card.anchoredPosition = Vector2.Lerp(transform.position, new Vector2((i * 100) -150, -i * 10), moveStep);
        yield return null; 
    }
    
    //Updates the text on the cards 
    private void UpdateCardData(ref TMP_Text[] data, int waitTime)
    {
        //update it! (it's almost like setting up a constructor! (we could make it like that, but why?))
        data[1] = waitTime.ToString(); 
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
