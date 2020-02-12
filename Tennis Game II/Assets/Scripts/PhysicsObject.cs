using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    [Header("References")]
    [SerializeField] protected Rigidbody rb;

    [Header("Base Movement")]
    [SerializeField] protected Vector3 targetDirection;

    [Header("Stats")]
    [SerializeField] protected Stats stats;

    [Header("Aim Target")]
    [SerializeField] protected GameObject aimTarget; 

    [Header("Cards")]

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>(); //gets a reference to gameObject's rigidbody
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetDirection = Move();
        rb.position += targetDirection;
    }

    protected virtual Vector3 Move(){ return Vector3.zero; } //moves player/ ai

}
