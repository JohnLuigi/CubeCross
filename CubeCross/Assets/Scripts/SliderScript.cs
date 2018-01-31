using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour {

    public float speed;
    public GameObject parentObject;

    public Vector3 origin;
    public GameObject yReference;
    //public Vector3 yReference;
    public Plane movePlane;

    public Vector3 pointOnPlane;
    public Vector3 oldPoint;   // distance to be used to see how far the slider was dragged along a plane in a frame
    public float distanceToTravel;
    public bool unclicked = true;


	// Use this for initialization
	void Start ()
    {
        transform.parent = parentObject.transform;

        // set the starting point to be moved towards when dragging away from
        origin = transform.position;
        yReference = new GameObject();
        yReference.transform.position = new Vector3(0, 1, 0);
        movePlane = new Plane(transform.parent.position, transform.position, yReference.transform.position);

        yReference.transform.parent = parentObject.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // update the location of the intersecting plane each frame so that its consistent with rotation
        movePlane = new Plane(transform.parent.position, transform.position, yReference.transform.position);
    }

    // make the distance to travel be the same as the mouse horizontal traversal distance from the
    // last frame

    private void OnMouseOver()
    {
        // make sure the slider has a parent, in this case the GameManager
        if (transform.parent != null)
        {
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

                    /*
                    // compare the travel vector to the vector between the mouseClick and the center of the puzzle
                    // if they have the same normalized direction, the mouse is sliding towards the center of the puzzle
                    float travelMagnitude = travelDirection.magnitude;
                    Vector3 travelNormalized = travelDirection / travelMagnitude;

                    float mouseMagnitude = mouseDirection.magnitude;
                    Vector3 mouseNormalized = mouseDirection / mouseMagnitude;
                    */

                    // find the angle between the direction the mouse moved in and the direction of the last drag on
                    // the plane
                    float cosAngle = Vector3.Dot(mouseDirection.normalized, travelDirection.normalized);
                    //Debug.Log("angle " + cosAngle);
                    // The closer the angles are, the closer their value will be to 1
                    // Opposite vectors will be -1 and orthogonal vectors will be 0

                    float step = travelDirection.magnitude;
                    // if the directions are in the same direction
                    // MOVE TOWARDS THE CUBES
                    if(1.0f - cosAngle <= 0.0009 )
                    {
                        //cosAngle is practically 1, so it is towards
                        Debug.Log("moving towards the center");
                        step *= -1.0f;
                        //step = travelDirection.magnitude;
                    }
                    // if the directions are away from each other
                    // MOVE AWAY FROM THE CUBES
                    else if(1.0 - cosAngle >= 1.99)
                    {
                        // cosAngle is practically -1, so it is away from the cube
                        Debug.Log("moving away from the center");

                        //step = travelDirection.magnitude;
                        //step *= -1.0f;

                    }

                    // if moving towards the center of the puzzle
                    //if(travelNormalized)

                    // if moving away from the puzzle


                    transform.position = Vector3.MoveTowards(transform.position, parentObject.transform.position,
                        step);

                    // save the point after translating the slider
                    oldPoint = pointOnPlane;
                }

                

                /*
                            float step = speed * Time.deltaTime;
                            transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, step);

                */
            }                
            
        }
    }

    private void OnMouseExit()
    {
        unclicked = true;
    }
}
