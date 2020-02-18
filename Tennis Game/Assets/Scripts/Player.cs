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

    private void FixedUpdate()
    {
        ClampPosition(); 
    }

    protected override void Move(Vector3 targetDirection)
    {
        //move player
        rb.velocity = targetDirection * moveSpeed * Time.deltaTime;
    }

    protected override void hitBall(GameObject ball)
    {
        //if we succssesfully hit the ball 
        if (Input.GetMouseButton(0))
            StartCoroutine(MoveBall(ball)); 
    }
}
