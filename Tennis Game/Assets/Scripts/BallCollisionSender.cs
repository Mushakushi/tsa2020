using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//lets TennisPlayer know if it is outside of a collider or not, for specifc use with MoveBall(); 
public class BallCollisionSender : MonoBehaviour
{
    TennisPlayer tpScript = new TennisPlayer();

    //can't continue to be moved
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Court Floor"))
            tpScript.canMoveBall = false;
    }

    //can continue to be moved 
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Court Floor"))
        {
            tpScript.canMoveBall = true;
            Debug.LogError("Moving out of court floor"); 
        }
    }
}
