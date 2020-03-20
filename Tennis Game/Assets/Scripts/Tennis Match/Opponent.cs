using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TennisPlayer
{

    protected override void ComputeDirection()
    {
        //Follow ball @ certain speed 
        if (canMove)
        {
            Vector3 move = Vector3.MoveTowards(transform.position, ball_rb.position - transform.position, moveSpeed);
            move.y = 0f;
            targetDirection = move;
        }
        else
        {
            targetDirection = Vector3.zero; 
        }
    }

    protected override void HitBall(Rigidbody ball, Vector3[] path, Effect effect)
    {
        //hit the ball 
        if (!isMoveBallRunning && canMove)
            StartCoroutine(MoveBall(ball_rb, path, effect));

        //the opponent likes to go crazy sometimes
        Utility.WaitFor(0.1f); 
    }
}
