using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : TennisPlayer
{
    private void Update()
    {
        targetDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //if we're actually moving ... move!
        if (targetDirection.magnitude > 0f)
            Move(targetDirection);
    }

    protected override void Move(Vector3 targetDirection)
    {
        ClampPosition();
        //move player
        rb.velocity = targetDirection * moveSpeed * Time.deltaTime;
    }

    protected override void hitBall(Rigidbody ball)
    {
        //if we succssesfully hit the ball 
        if (Input.GetMouseButton(0))
            StartCoroutine(MoveBall(ball)); 
    }
}
