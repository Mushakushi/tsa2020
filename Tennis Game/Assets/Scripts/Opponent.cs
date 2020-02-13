using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TennisPlayer
{
    [Header("Ball")]
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform playerAimTarget; 

    private void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        playerAimTarget = GameObject.FindGameObjectWithTag("AimTarget").transform; 
    }

    private void FixedUpdate()
    {
        ClampPosition(); 

        //Follow ball on the x @ certain speed 
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(ball.transform.position.x, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime); 
    }

    protected override void hitBall(Rigidbody ball_rb, Vector3 dir)
    {
        //hit the ball 
        ball_rb.velocity = dir.normalized * hitForce + new Vector3(0, 3, 0);
    }
}
