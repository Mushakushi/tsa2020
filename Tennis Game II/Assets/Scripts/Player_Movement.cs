using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : PhysicsObject
{

    protected override Vector3 Move()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * stats.moveSpeed;  
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Vector3 dir = aimTarget.transform.position - transform.position;

            if (Input.GetMouseButtonDown(0))
                other.GetComponent<Rigidbody>().velocity = dir.normalized * stats.hitForce + new Vector3(0, 5, 0);
        }
    }

}
