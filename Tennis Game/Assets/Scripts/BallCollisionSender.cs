using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionSender : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Court Floor"))
        {
            TennisPlayer.isBallGrounded = true; 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Court Floor"))
        {
            TennisPlayer.isBallGrounded = false; 
        }
    }
}
