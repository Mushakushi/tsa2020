using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : TennisPlayer
{
    [Header("AI Aim")]
    [SerializeField] private Transform playerAimTarget; 

    private void Start()
    {
        playerAimTarget = GameObject.FindGameObjectWithTag("AimTarget").transform; 
    }

    private void FixedUpdate()
    {
        ClampPosition(); 

        //Follow ball on the x @ certain speed 
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(ball.transform.position.x, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime); 
    }

    protected override void hitBall(GameObject ball)
    {
        //hit the ball 
        StartCoroutine(MoveBall(ball)); 
    }
}
