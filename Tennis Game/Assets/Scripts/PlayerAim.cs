using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{

    [SerializeField] private Vector3 mousePosition;
    [SerializeField] private float followSpeed; 

    // Update is called once per frame
    void Update()
    {
        //remeber: always initialize your variables before you use them in a function!
        mousePosition = -Vector3.one; 
        Plane plane = new Plane(Vector3.up, 0f);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distanceToPlane;

        if (plane.Raycast(ray, out distanceToPlane))
            mousePosition = ray.GetPoint(distanceToPlane); 
    }

    private void FixedUpdate()
    {
        //move to clamped bounds
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(Mathf.Clamp(mousePosition.x, -1, 1),  transform.position.y,
            Mathf.Clamp(mousePosition.z, 0.1f, 2f)), followSpeed);
    }

    //https://www.youtube.com/watch?v=RGjojuhuk_s for help with code 
}
