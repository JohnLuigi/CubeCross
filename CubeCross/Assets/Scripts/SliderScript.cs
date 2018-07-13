using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour {

    //public float speed;
    private GameObject parentObject;
    private CubeManager parentManager;

    private Vector3 startingDistance;
    private GameObject axisReference;       // objects used to help define the plane the slider will move along
    private GameObject axisReference2;  // reference 2 will be at the same y-value as the center of the puzzle
    //public Vector3 yReference;
    private Plane movePlane;
    private GameObject originalLocation;

    private Vector3 pointOnPlane;
    private Vector3 oldPoint;   // distance to be used to see how far the slider was dragged along a plane in a frame
    private float distanceToTravel;
    private bool unclicked = true;

    private float clickTime = 0f;
    private float timeTracker = 0f;
    private float doubleClickDelay = 0.2f;  // default double click is half a second on windows

    public bool sliding = false;   // use this to make sure cube deletion doesn't occur while using the sliders

    private float closestDistance;

    private GameObject otherSlider;
// private int puzzSize;

//private float halfCubeDist;
    private float halfCubeDist_X;
    private float halfCubeDist_Y;
    private float halfCubeDist_Z;

    private GameObject puzzleSelector;  // reference to the puzzle selection UI element

    private bool puzzleInitialized = false;

    //public bool isSliding = false;
    // TODO
    // update the uses of 8.5f as a starting distance for the sliders to be some function of
    // the respective dimension of the puzzle


    // Use this for initialization
    void Start ()
    {
        puzzleSelector = GameObject.Find("PuzzleSelector");

        // initially hide the slider until it is activated when the puzzle is created
        gameObject.SetActive(false);


        
        
/*
        parentObject = GameObject.Find("GameManager");
        transform.parent = parentObject.transform;

        parentManager = parentObject.GetComponent<CubeManager>();

    //puzzSize = parentManager.puzzleSize;
    //halfCubeDist = (puzzSize / 2f) - 0.5f;

        halfCubeDist_X = (parentManager.puzzleSize_X / 2f);
        halfCubeDist_Y = (parentManager.puzzleSize_Y / 2f);
        halfCubeDist_Z = (parentManager.puzzleSize_Z / 2f);

        // set the reference point to be used to define the plane that will contain the slider
        // also set the starting position of the slider
        if (gameObject.name == "XSlider")
        {
            transform.position = new Vector3(halfCubeDist_X + 2, 0, halfCubeDist_Z + 1);

            // this reference is on the same edge of the cube as the X Slider, but with an increased y-value (high ref)
            axisReference = new GameObject { name = "XSliderReference1"};
            axisReference.transform.position = new Vector3(0, halfCubeDist_Y, halfCubeDist_Z + 1);
            axisReference.transform.parent = parentObject.transform;

            // this reference is on the same y-value as the slider, and same z-value (even ref)
            axisReference2 = new GameObject { name = "XSliderReference2" };
            axisReference2.transform.position = new Vector3(-halfCubeDist_X, 0, halfCubeDist_Z + 1);
            axisReference2.transform.parent = parentObject.transform;

            otherSlider = GameObject.Find("ZSlider");
        }
        else if(gameObject.name == "ZSlider")
        {
            transform.position = new Vector3(-halfCubeDist_X - 1, 0, -halfCubeDist_Z - 2);

            // this reference is on the same edge of the cube as the Z Slider, but with an increased y-value (high ref)
            axisReference = new GameObject { name = "ZSliderReference1" };
            axisReference.transform.position = new Vector3(-halfCubeDist_X - 1, halfCubeDist_Y, 0);
            axisReference.transform.parent = parentObject.transform;

            // this reference is on the same y-value as the Z slider, and same x-value (even ref)
            axisReference2 = new GameObject { name = "ZSliderReference2" };
            axisReference2.transform.position = new Vector3(-halfCubeDist_X - 1, 0, halfCubeDist_Z);
            axisReference2.transform.parent = parentObject.transform;

            otherSlider = GameObject.Find("XSlider");
        }
        

        movePlane = new Plane(axisReference2.transform.position, transform.position, axisReference.transform.position);

        // store a gameobject that is parented to the gameManager cube puzzle
        originalLocation = new GameObject { name = "OriginalLocation"};
        originalLocation.transform.position = transform.position;
        originalLocation.transform.parent = parentObject.transform;

        // record the original distance from the center of the puzzle + offset (axisReference2)
        // this wil be the maximum distance the slider can be away from the puzzle
        startingDistance = transform.position - axisReference2.transform.position;
        // might need to swap the order of this subtraction

        */

        //////////////////////////////////////
        /// END OF ORIGINAL START BLOCK///////
        //////////////////////////////////////

        /*
        // point between the slider and the center of the cube that the cube cannot move any closer
        // for the xSlider, it will be along the x-axis, located at theshold-puzzleSize-0.5 units
        float tempThresh = parentObject.GetComponent<CubeManager>().hideThreshold_X;
        float tempSize = parentObject.GetComponent<CubeManager>().puzzleSize_X;
        closestDistance = (tempThresh - tempSize) - 1f;

        closestDistance = 1f;
        
        // if this is the ZSlider, set these values according to the z-values in the parent object
        if(gameObject.name == "ZSlider")
        {
            tempThresh = parentObject.GetComponent<CubeManager>().hideThreshold_Z;
            tempSize = parentObject.GetComponent<CubeManager>().puzzleSize_Z;
            closestDistance = (-tempThresh + tempSize) + 1f;

            closestDistance = -1f;
        }
        */
        //TODO
        // see if closest distance is being stopped in this script
        closestDistance = 0.5f;

    }

    // Update is called once per frame
    void Update ()
    {
        // if the puzzle selection menu is active, don't make the game interactive
        if (puzzleSelector.activeSelf)
        {
            return;
        }

        // don't do anything until the puzzle has been initialized
        if (!puzzleInitialized)
            return;

        // update the location of the intersecting plane each frame so that it's consistent with rotation
        movePlane = new Plane(axisReference2.transform.position, transform.position, axisReference.transform.position);

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
        // if the puzzle selection menu is active, don't make the game interactive
        if (puzzleSelector.activeSelf)
        {
            return;
        }

        // don't do anything until the puzzle has been initialized
        if (!puzzleInitialized)
            return;
        // TODO
        // take a look at this "otherSlider != null" check and see why it was necessary to make the game not
        // call a null reference when mousing over the XSlider

        // this prevents the slider from doing anything if another slider is in use
        if (otherSlider != null && otherSlider.GetComponent<SliderScript>().sliding == true)
        {
            return;
        }

        // make sure the slider has a parent, in this case the GameManager
        if (transform.parent != null)
        {
            // double click code to reset the position of the mouse
            if(Input.GetMouseButtonUp(0))
            {
                sliding = false;

                float oldClickTime = clickTime;

                clickTime = timeTracker;

                float timeSinceLastClick = clickTime - oldClickTime;

                // if the last click was also on the cube
                if(timeSinceLastClick <= doubleClickDelay && timeSinceLastClick != 0f)
                {
                    //reset the position of the slider
                    transform.position = originalLocation.transform.position;
                    // make all cubes visible again
                    parentManager.ShowAllCubes();
                    return;
                }
            }

            // if the left mouse button is held over the slider controller
            if(Input.GetMouseButton(0))
            {
                sliding = true;

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
                    unclicked = false;
                }
                else
                {
                    // vector between the last two mouse clicks on the plane
                    Vector3 travelDirection = pointOnPlane - oldPoint;

                    // vector between mouse and center of puzzle + offset (axisReference2)
                    Vector3 mouseDirection = pointOnPlane - axisReference2.transform.position;

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
                    transform.position = Vector3.MoveTowards(transform.position, axisReference2.transform.position,
                    step);
                    

                    // clamp the movement to not go past the starting distance from the cube
                    Vector3 currentDistance = transform.position - axisReference2.transform.position;

                    // change the comparisons based on the slider (since the Z axis values are going to be dealing with negatives)
                    bool compDist = (currentDistance.magnitude > startingDistance.magnitude);

                    bool compClose = (currentDistance.magnitude < closestDistance);

                    if (compDist)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, axisReference2.transform.position,
                    -step);
                        //Debug.Log("too far");
                        //return;
                    }
                    else if(compClose)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, axisReference2.transform.position,
                    -step);
                        //Debug.Log("too close");
                        //return;
                    }

                    // clamp the movement to not go too close to the center of the puzzle

                    // save the point after translating the slider
                    oldPoint = pointOnPlane;
                }
            }
        }
    }

    private void OnMouseExit()
    {
        unclicked = true;
        sliding = false;
    }

    // public version of the code that was originally part of the start method
    public void Initialize()
    {
        puzzleSelector = GameObject.Find("PuzzleSelector");

        parentObject = GameObject.Find("GameManager");
        transform.parent = parentObject.transform;

        parentManager = parentObject.GetComponent<CubeManager>();

        //puzzSize = parentManager.puzzleSize;
        //halfCubeDist = (puzzSize / 2f) - 0.5f;

        halfCubeDist_X = (parentManager.puzzleSize_X / 2f);
        halfCubeDist_Y = (parentManager.puzzleSize_Y / 2f);
        halfCubeDist_Z = (parentManager.puzzleSize_Z / 2f);

        // The length of the box collider (set to encapsulate the entire model),
        // to be used to set the exact position the sliders will rest from the puzzle.
        float colliderXDist = this.GetComponent<BoxCollider>().size.x / 2f;

        // set the reference point to be used to define the plane that will contain the slider
        // also set the starting position of the slider
        if (gameObject.name == "XSlider")
        {
            // set the position of the sliders in relation to the main cube body
            transform.position = new Vector3(halfCubeDist_X + 1 + colliderXDist, 0, halfCubeDist_Z + 1);
            // fix the rotation when the sliders are moved to the positions in relation to the main cube body
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -45f, transform.localEulerAngles.z);

            // this reference is on the same edge of the cube as the X Slider, but with an increased y-value (high ref)
            axisReference = new GameObject { name = "XSliderReference1" };
            axisReference.transform.position = new Vector3(0, halfCubeDist_Y, halfCubeDist_Z + 1);
            axisReference.transform.parent = parentObject.transform;

            // this reference is on the same y-value as the slider, and same z-value (even ref)
            axisReference2 = new GameObject { name = "XSliderReference2" };
            axisReference2.transform.position = new Vector3(-halfCubeDist_X, 0, halfCubeDist_Z + 1);
            axisReference2.transform.parent = parentObject.transform;

            otherSlider = GameObject.Find("ZSlider");
        }
        else if (gameObject.name == "ZSlider")
        {
            // set the position of the sliders in relation to the main cube body
            transform.position = new Vector3(-halfCubeDist_X - 1, 0, -halfCubeDist_Z -1 - colliderXDist);
            // fix the rotation when the sliders are moved to the positions in relation to the main cube body
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -45f, transform.localEulerAngles.z);

            // this reference is on the same edge of the cube as the Z Slider, but with an increased y-value (high ref)
            axisReference = new GameObject { name = "ZSliderReference1" };
            axisReference.transform.position = new Vector3(-halfCubeDist_X - 1, halfCubeDist_Y, 0);
            axisReference.transform.parent = parentObject.transform;

            // this reference is on the same y-value as the Z slider, and same x-value (even ref)
            axisReference2 = new GameObject { name = "ZSliderReference2" };
            axisReference2.transform.position = new Vector3(-halfCubeDist_X - 1, 0, halfCubeDist_Z);
            axisReference2.transform.parent = parentObject.transform;

            otherSlider = GameObject.Find("XSlider");
        }


        movePlane = new Plane(axisReference2.transform.position, transform.position, axisReference.transform.position);

        // store a gameobject that is parented to the gameManager cube puzzle
        originalLocation = new GameObject { name = "OriginalLocation" };
        originalLocation.transform.position = transform.position;
        originalLocation.transform.parent = parentObject.transform;

        // record the original distance from the center of the puzzle + offset (axisReference2)
        // this wil be the maximum distance the slider can be away from the puzzle
        startingDistance = transform.position - axisReference2.transform.position;
        // might need to swap the order of this subtraction



        // now we can run the Update() method
        puzzleInitialized = true;
    }
}
