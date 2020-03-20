using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Contains functions and data to manipulate the score 
public class ScoreManager
{
    //notice there is no singleton (this is becuase this cannot be duplicated -- it's just a class!)

    //0 corresponds to love, 1 corresponds to 15 etc. 
    public static int playerPointIndex, opponentPointIndex;

    //how many sets has each player won (would it be better to store both the above and this in an int[]?)
    public static int playerSetsWon, opponentSetsWon;

    //are we in a deuce state?
    public static bool deuce = false; 

    //Increase the score
    public static int IncreaseScore(bool player)
    {
        if (!deuce)
        {
            //increase the appropriate score
            if (player)
                playerPointIndex++;
            else
                opponentPointIndex++;

            //check for deuce? (deuce if the score is 40-all)
            deuce = playerPointIndex == opponentPointIndex && playerPointIndex == 3;

            //return the scores 
            if (player)
                return playerPointIndex;
            else
                return opponentPointIndex; 
        }
        else
        {
            //increase the score
            if (player)
                playerPointIndex++;
            else
                opponentPointIndex++; 

            //if the player/ opponent caught up, "reset" deuce state
            if (playerPointIndex == opponentPointIndex)
            {
                playerPointIndex = 4;
                opponentPointIndex = 4; 
            }

            //return the apprporiate variable
            if (player)
                return playerPointIndex;
            else
                return opponentPointIndex; 
        }
    }

    public static int GetPlayerPoints()
    {
        return playerPointIndex; 
    }

    public static int GetOpponentPoints()
    {
        return opponentPointIndex; 
    }

    public static int[] GetPoints()
    {
        return new int[2] { playerPointIndex, opponentPointIndex }; 
    }

    public static int GetSetNumber()
    {
        //I don't have to store a setnumber variable, becuase this is that that really is...
        return playerSetsWon + opponentSetsWon; 
    }

    //In this special world of tennis, the opponent goes first! (this is just to emphasize the importance of the opponent!
    public static bool IsPlayerServing()
    {
        return GetSetNumber() % 2 == 1 ? true : false; 
    }

    public static bool IsSetWon()
    {
        //NORMAL: 
        //if player is on pointIndex 4 (scored on point 40 AND while opponent is on point 30, so they are not tied), player wins
        //DEUCE: 
        //if player is on pointIndex 6 (only achieved from being in a deuce state so there is no need to check there)
        if ((playerPointIndex == opponentPointIndex + 2 && playerPointIndex == 4) || playerPointIndex == 6)
        {
            NewSet(true); 
            return true;
        }
        //player gets the dub
        else if ((opponentPointIndex == playerPointIndex + 2 && opponentPointIndex == 4) || opponentPointIndex == 6)
        {
            NewSet(false); 
            return true;
        }
        else
            return false; 
    }

    //checks to see if the match is won
    public static bool IsMatchWonPlayer()
    {
        if (playerSetsWon == 6)
            return true;
        else
            return false; 
    }

    public static bool IsMatchWonOpponent()
    {
        if (playerSetsWon == 6)
            return true;
        else
            return false; 
    }

    //resets the points and sets up for the next set (kind of sounds like ... REset ... ha ha)
    private static void NewSet(bool player)
    {
        //deuce only ends when the game ends
        deuce = false; 

        //increase the sets won
        if (player)
            playerSetsWon++;
        else
            opponentSetsWon++;

        //reset the points
        playerPointIndex = 0;
        opponentPointIndex = 0; 
    }
}
