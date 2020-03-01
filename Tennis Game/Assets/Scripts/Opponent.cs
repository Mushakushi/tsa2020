using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TennisPlayer
{

    protected override void ComputeDirection()
    {
        //Follow ball @ certain speed 
        Vector3 move = Vector3.MoveTowards(transform.position, ball_rb.position - transform.position, moveSpeed);
        move.y = 0f; 
        targetDirection = move; 
    }

    protected override void hitBall(Rigidbody ball)
    {
        //hit the ball 
        StartCoroutine(MoveBall(ball_rb)); 
    }
}
