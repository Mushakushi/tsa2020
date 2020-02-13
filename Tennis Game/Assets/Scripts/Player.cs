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

    protected override void hitBall(Rigidbody ball_rb, Vector3 dir)
    {
        if (Input.GetMouseButtonDown(0))
        {
            //hit the ball 
            ball_rb.velocity = dir.normalized * hitForce + new Vector3(0, 3, 0);
        }
    }

    protected override void Move(Vector3 targetDirection)
    {
        //move player
        rb.velocity = targetDirection * moveSpeed * Time.deltaTime;
    }
}
