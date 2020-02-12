using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleFollow : MonoBehaviour
{

    [Header("Mouse Position")]
    [SerializeField] private Vector3 mousePos_world;

    [Header("Boundaries")]
    [SerializeField] private float max_x;
    [SerializeField] private float min_x;
    [SerializeField] private float max_z;
    [SerializeField] private float min_z; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Plane plane = new Plane(Vector3.up, 0f);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distanceToPlane; 

        if (plane.Raycast(ray, out distanceToPlane))
        {
            mousePos_world = ray.GetPoint(distanceToPlane); 
        }
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Clamp(mousePos_world.x, min_x, max_x), transform.position.y, Mathf.Clamp(mousePos_world.z, min_z, max_z)); 
    }
}
