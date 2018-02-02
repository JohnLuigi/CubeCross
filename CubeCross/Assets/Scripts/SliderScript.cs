using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour {

    public float speed;
    public GameObject parentObject;

    public Vector3 startingDistance;
    public GameObject yReference;
    //public Vector3 yReference;
    public Plane movePlane;
    public GameObject originalLocation;

    public Vector3 pointOnPlane;
    public Vector3 oldPoint;   // distance to be used to see how far the slider was dragged along a plane in a frame
    public float distanceToTravel;
    public bool unclicked = true;

    private float clickTime = 0f;
    private float timeTracker = 0f;
    private float doubleClickDelay = 0.2f;  // default double click is half a second on windows

    //public bool isSliding = false;


	// Use this for initialization
	void Start ()
    {
        transform.parent = parentObject.transform;

        // set the starting point to be moved towards when dragging away from
        yReference = new GameObject();
        yReference.transform.position = new Vector3(0, 1, 0);
        yReference.transform.parent = parentObject.transform;

        movePlane = new Plane(transform.parent.position, transform.position, yReference.transform.position);

        // store a gameobject that is parented to the gameManager cube puzzle
        originalLocation = new GameObject();
        originalLocation.transform.position = transform.position;
        originalLocation.transform.parent = parentObject.transform;

        // record the original distance from the center of the puzzle (the parent)
        // this wil be the maximum distance the slider can be away from the puzzle
        startingDistance = transform.position - parentObject.transform.position;
        // might need to swap the order of this subtraction
    }
	
	// Update is called once per frame
	void Update ()
    {
        // update the location of the intersecting plane each frame so that its consistent with rotation
        movePlane = new Plane(transform.parent.position, transform.position, yReference.transform.position);

        if(Input.GetMouseButtonUp(0))
        {
            unclicked = true;
        }

        timeTracker += Time.deltaTime;
/*
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.transform.gameObject.tag == "Slider")
                {
                    // we've hit the slider, now lets find its location on the plane
                }
            }
        }
*/
    }

    // make the distance to travel be the same as the mouse horizontal traversal distance from the
    // last frame

    private void OnMouseOver()
    {
        // make sure the slider has a parent, in this case the GameManager
        if (transform.parent != null)
        {
            // double click code to reset the position of the mouse
            if(Input.GetMouseButtonUp(0))
            {
                float oldClickTime = clickTime;

                clickTime = timeTracker;

                float timeSinceLastClick = clickTime - oldClickTime;

                // if the last click was also on the cube
                if(timeSinceLastClick <= doubleClickDelay && timeSinceLastClick != 0f)
                {
                    //reset the position of the slider
                    transform.position = originalLocation.transform.position;
                    return;
                }
            }

            // if the left mouse button is held over the slider controller
            if(Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float newDistance;
                if (movePlane.Raycast(ray, out newDistance))
                {
                    //Vector3 pointOnPlane = ray.origin + (ray.direction * newDistance);
                    pointOnPlane = ray.GetPoint(newDistance);
                }

                // the first movement, where the oldPoint is undefined
                if(unclicked)
                {
                    // just save the location hit, since it's the first time the slider is hit it can't move until
                    // it is dragged
                    oldPoint = pointOnPlane;
                    Debug.Log("first click");
                    unclicked = false;
                }
                else
                {
                    // vector between the last two mouse clicks on the plane
                    Vector3 travelDirection = pointOnPlane - oldPoint;

                    // vector between mouse and center of puzzle
                    Vector3 mouseDirection = pointOnPlane - parentObject.transform.position;

                    // find the angle between the direction the mouse moved in and the direction of the last drag on
                    // the plane
                    float cosAngle = Vector3.Dot(mouseDirection.normalized, travelDirection.normalized);

                    // The closer the angles are, the closer their value will be to 1
                    // Opposite vectors will be -1 and orthogonal vectors will be 0

                    float step = travelDirection.magnitude;
                    float modifier = 1.0f;
                    // if the directions are in the same direction
                    // MOVE TOWARDS THE CUBES
                    if(1.0f - cosAngle <= 0.0009 )
                    {
                        //cosAngle is practically 1, so it is towards the cube
                        step *= -modifier;
                    }
                    // if the directions are away from each other
                    // MOVE AWAY FROM THE CUBES
                    else if(1.0 - cosAngle >= 1.99)
                    {
                        // cosAngle is practically -1, so it is away from the cube
                        step *= modifier;
                    }

                    // if moving towards the center of the puzzle
                    //if(travelNormalized)

                    // if moving away from the puzzle                    
                    transform.position = Vector3.MoveTowards(transform.position, parentObject.transform.position,
                    step);
                    

                    // clamp the movement to not go past the starting distance from the cube
                    Vector3 currentDistance = transform.position - parentObject.transform.position;
                    if (currentDistance.magnitude > startingDistance.magnitude)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, parentObject.transform.position,
                    -step);
                        Debug.Log("too far");
                        //return;
                    }

                    // save the point after translating the slider
                    oldPoint = pointOnPlane;
                }
            }
        }
    }

    private void OnMouseExit()
    {
        unclicked = true;
    }
}
