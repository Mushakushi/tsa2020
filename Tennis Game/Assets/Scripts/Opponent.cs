using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TennisPlayer
{

    private void FixedUpdate()
    {
        ClampPosition(); 

        //Follow ball on the x @ certain speed 
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(ball_rb.position.x, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime); 
    }

    protected override void hitBall(Rigidbody ball)
    {
        //hit the ball 
        StartCoroutine(MoveBall(ball)); 
    }
}
