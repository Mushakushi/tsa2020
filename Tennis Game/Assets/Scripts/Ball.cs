using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movement")]
    public bool isMoving;
    public Rigidbody rb;
    public Vector3 velocity;
    public int bounces; 

    [Space]
    [SerializeField] private float minBounceVelocity; 
    [SerializeField] private float bounceHieghtMultiplier;
    [SerializeField] private float bounceDistanceMultiplier;

    [Header("Collision Detection")]
    public bool isGrounded;
    [SerializeField] private RaycastHit hitInfo;
    [SerializeField] private float distance; //distance to collider
    [SerializeField] private float length;
    [SerializeField] private float shellRadius; 

    [Header("Gravity")]
    [SerializeField] private float gravityModifier;

    private void Awake()
    {
        GameManager.instance.onSetEnd += OnSetEnd;
        GameManager.instance.onSetStart += OnSetStart; 
    }

    #region Events
    //On set end 
    private void OnSetEnd()
    {
        //Hold the ball!
        velocity = Vector3.zero;
        isMoving = true;
        bounces = 0;
    }

    //On set start 
    private void OnSetStart()
    {
        isMoving = false;
    }

    private void Start()
    {
        //not moving until proven otherwise 
        isMoving = false;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    //clean code. clean code. 
    private void OnDestroy()
    {
        GameManager.instance.onSetEnd -= OnSetEnd;
        GameManager.instance.onSetStart -= OnSetStart; 
    }
    #endregion  

    #region Kinematic Physics
    //custom behavior for our very realistic ball
    private void FixedUpdate()
    {
        //grounded is false until proven otherwise 
        isGrounded = false;
        //if we not being slapped by a racket fall; if we're being hit, freeze (the computations...)!
        velocity = !isMoving ? velocity /*are we bouncing?*/ + gravityModifier * Physics.gravity : Vector3.zero;

        //if we're falling naturally, do some calculations to get us falling and colliding 
        if (velocity.magnitude > 0.001f && !isMoving)
        {
            distance = velocity.magnitude; //length of direction that we are trying to move 
            Physics.Raycast(rb.position, Vector3.down, out hitInfo, length);
            //if we're hitting the ground 
            if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Court Floor"))
            {
                //normal (how the line reflects) of the ground surface
                isGrounded = true;

                //if we're grounded, bounce (but not infinitely, once we slow down, let's roll)
                if (velocity.magnitude > minBounceVelocity)
                {
                    //add back velocity so that ball can bounce -- and not smack on the ground (don't divide by zero; just don't)
                    float bounceLoss = bounces == 0 ? 1f : 1f / bounces; 
                    velocity = new Vector3(velocity.x, bounceHieghtMultiplier * bounceLoss, bounceDistanceMultiplier * velocity.z);
                    bounces++; //the ball has bounced
                }

                //distance to the collider 
                float modifiedDistance = hitInfo.distance - shellRadius;
                //if we're about to move into a collider and we don't have enough velocity to bounce (check if we're falling), adjust our velocity to not hit it 
                if (distance > modifiedDistance)
                {
                    distance = modifiedDistance;
                    if (velocity.magnitude > minBounceVelocity && velocity.y > 0)
                    {
                        distance = velocity.magnitude; 
                    }
                }
            }

            //if we're not being moved by the tennis racket, then move according to normal gravity and physics bruh 
            Vector3 moveTo = velocity.normalized /*sheer direction*/ * distance /*"magnitude"*/ * Time.fixedDeltaTime /*smooth*/;
            if (!float.IsNaN(moveTo.x) && !float.IsNaN(moveTo.y) && !float.IsNaN(moveTo.z))//is valid assignment? I get this error sometimes
                rb.position += moveTo; //MOVE!!!
        }
        //if you're being moved or going to slow, don't velocitize ... yo!

        //an adapted version of Unity's kinematic Rigidbody Platfomer Character Controller tutorial 
        //https://learn.unity.com/tutorial/live-session-2d-platformer-character-controller#5c7f8528edbc2a002053b695
    }
    #endregion

    #region debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, length, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.up * distance)); 
    }
    #endregion
}
