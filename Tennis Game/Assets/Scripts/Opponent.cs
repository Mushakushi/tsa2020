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

    protected override void HitBall(Rigidbody ball, Vector3[] path, Effect effect)
    {
        //hit the ball 
        if (!isMoveBallRunning)
            StartCoroutine(MoveBall(ball_rb, path, effect)); 
    }
}
