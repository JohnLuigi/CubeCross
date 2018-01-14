using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))] 
public class CameraMovement : MonoBehaviour {

    //todo
    // change this to an array or some other structure
    public List<Transform> targets;
    private Vector3 centerPoint;

    public Vector3 offset;

    private Vector3 newPosition;
    private Vector3 velocity;

    public float smoothTime = 0.5f;

    public float minZoom = 40.0f;
    public float maxZoom = 10.0f;

    // this is the maximum possible distance that two objects can be from one another
    // in this case, the maximum index for the array holding the cubes (default is 10)
    public float maxDistance = 10.0f;

    private Camera cam;


    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate ()
    {
        if (targets.Count == 0)
            return;
        Move();
        Zoom();
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / maxDistance);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    void Move()
    {
        centerPoint = GetCenterPoint();             // set the center point to be the average center
        newPosition = centerPoint + offset;
        // set the camera's center to be the average center
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    Vector3 GetCenterPoint()
    {
        // if only one target, just return the one item to be centered on
        if(targets.Count == 1)        
            return targets[0].position;

        // this creates a Bounds class that will contain the average center based on all the items in the list
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for(int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        // return the overall center
        return bounds.center;
        
    }
}
