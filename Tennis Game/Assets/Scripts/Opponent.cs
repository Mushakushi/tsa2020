using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TennisPlayer
{

    protected override void ComputeDirection()
    {
        //Follow ball on the x @ certain speed 
        targetDirection = new Vector3((ball.transform.position - transform.position).x, 0f, 0f); 
    }

    protected override void hitBall(GameObject ball)
    {
        //hit the ball 
        StartCoroutine(MoveBall(ball)); 
    }
}
