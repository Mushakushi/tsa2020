using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TennisPlayer
{

    protected override void ComputeDirection()
    {
        //Follow ball on the x @ certain speed 
        targetDirection = Vector3.MoveTowards(transform.position,
            new Vector3(ball_rb.position.x, transform.position.y, transform.position.z), moveSpeed); 
    }

    protected override void hitBall(Rigidbody ball)
    {
        //hit the ball 
        StartCoroutine(MoveBall(ball)); 
    }
}
