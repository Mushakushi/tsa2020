using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : TennisPlayer
{
    //Compute the movement direction 
    protected override void ComputeDirection()
    {
        targetDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * moveSpeed; 
    }

    protected override void hitBall(Rigidbody ball_rb)
    {
        //if we succssesfully hit the ball 
        if (Input.GetMouseButton(0))
            StartCoroutine(MoveBall(ball_rb, deck[0])); 
    }
}
