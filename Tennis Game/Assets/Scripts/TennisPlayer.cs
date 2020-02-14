using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisPlayer : MonoBehaviour
{
    //note: player in this script refers to the tennis player, not just the player-player
    [Header("Stats")]
    [SerializeField] protected float hitForce;
    [SerializeField] protected float moveSpeed;

    [Header("Aiming")]
    [SerializeField] protected Transform aimTarget;

    [Header("Movement")]
    [SerializeField] protected Vector3 targetDirection; 

    [Header("RigidBody")]
    [SerializeField] protected Rigidbody rb; 

    // Start is called before the first frame update
    void Start()
    { 
        rb = gameObject.GetComponent<Rigidbody>(); //rigidbody component
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //Moves player
        Move(targetDirection); 
    }

    //ai and player move differently 
    protected virtual void Move(Vector3 targetDirection) { }

    //Clamp positions to respective halves 
    protected void ClampPosition()
    {
        //clamp tennis players to bounds based on whothey are 
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1, 1), transform.position.y,
            Mathf.Clamp(transform.position.z, gameObject.CompareTag("Opponent") ? 0f : -2f,
            gameObject.CompareTag("Opponent") ? 2f : 0f));
    }

    //return the ball. This is put inside of OnTriggerStay becuase each of these classes will use it anyways!
    private void OnTriggerStay(Collider other)
    {
        //is this the ball that we are hitting? 
        if (other.CompareTag("Ball"))
        {
            //get rigidbody 
            Rigidbody ball_rb = other.GetComponent<Rigidbody>();
            //direction that we are hitting the ball 
            Vector3 dir = aimTarget.position - transform.position; 

            hitBall(ball_rb, dir);
        }
    }

    //Child classes will define how the process of hitting the ball works 
    protected virtual void hitBall(Rigidbody ball, Vector3 direction) { }
}
