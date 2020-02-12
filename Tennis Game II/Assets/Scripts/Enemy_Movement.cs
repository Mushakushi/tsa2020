using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : PhysicsObject
{

    [Header("Ball")]
    [SerializeField] protected GameObject boru; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(boru.transform.position.x, transform.position.y, transform.position.z); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Vector3 dir = aimTarget.transform.position - transform.position;
            other.GetComponent<Rigidbody>().velocity = dir.normalized * stats.hitForce + new Vector3(0, 5, 0);
        }
    }
}
