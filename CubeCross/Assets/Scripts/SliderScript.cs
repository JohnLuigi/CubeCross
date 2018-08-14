using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour {

    //public float speed;
    private GameObject parentObject;
    private CubeManager parentManager;

    private Vector3 startingPoint;
    private GameObject axisReference;       // objects used to help define the plane the slider will move along
    private GameObject axisReference2;  // reference 2 will be at the same y-value as the center of the puzzle
    //public Vector3 yReference;
    private Plane movePlane;
    public GameObject originalLocation;

    private Vector3 pointOnPlane;
    private bool unclicked = true;

    private float clickTime = 0f;
    private float timeTracker = 0f;
    private float doubleClickDelay = 0.2f;  // default double click is half a second on windows

    public bool sliding = false;   // use this to make sure cube deletion doesn't occur while using the sliders

    //private float closestDistance;

    public GameObject otherSlider;

    public int distanceInt; // This value will be read by the cube manager to determine how far it is from the puzzle
    
    private float halfCubeDist_X;
    private float halfCubeDist_Y;
    private float halfCubeDist_Z;

    private GameObject puzzleSelector;  // reference to the puzzle selection UI element

    private bool puzzleInitialized = false;

    // This value will store the Z distance for the x slider to always be at,
    // or the X distance for the Z slider to always be at.
    private float axisDistance;
    
    // TODO
    // update the uses of 8.5f as a starting distance for the sliders to be some function of
    // the respective dimension of the puzzle


    // Use this for initialization
    void Start ()
    {
        puzzleSelector = GameObject.Find("PuzzleSelector");

        // initially hide the slider until it is activated when the puzzle is created
        gameObject.SetActive(false);

        if (name == "XSlider" && otherSlider == null)
        {
            otherSlider = GameObject.Find("ZSlider");
        }

    }
    //TODO
    // Make the other slider reappear when the slider being used is returned to its starting position

    //TODO
    // Sliders working mostly as intended, but while dragging them they do weird stuff.
    // Sometimes they fly far off into the distance
    // When dragging, they sometimes don't move as many units as they should

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log(name + " is at localPos" + transform.localPosition);

        // if the puzzle selection menu is active, don't make the game interactive
        if (puzzleSelector.activeSelf)
        {
            return;
        }

        // don't do anything until the puzzle has been initialized
        if (!puzzleInitialized)
            return;

        // update the location of the intersecting plane each frame so that it's consistent with rotation
        movePlane = new Plane(axisReference2.transform.position, originalLocation.transform.position,
            axisReference.transform.position);

        // if the left mouse button is held over the slider controller
        if (Input.GetMouseButton(0) && sliding == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float newDistance;
            if (movePlane.Raycast(ray, out newDistance))
            {
                //Vector3 pointOnPlane = ray.origin + (ray.direction * newDistance);
                pointOnPlane = ray.GetPoint(newDistance);
                
            }

            // The first movement where the slider hasn't been interacted with
            if (unclicked)
            {
                // Set the slider to have been clicked.
                unclicked = false;
            }
            else
            {
                // The InverseTransformPoint function compares an input Vector3 to an object's
                // transform. The Vector3 that is set by this function determines where the input object
                // is located in comparison to the function's attached transform.
                Vector3 pointRelative = parentObject.transform.InverseTransformPoint(pointOnPlane);

                //Debug.Log("Slider is " + pointRelative.x + "Units away from the puzzle");

                float maxDistance = startingPoint.magnitude - halfCubeDist_X;
                float minDistance = -halfCubeDist_X;
                if (name.Equals("ZSlider"))
                {
                    maxDistance = startingPoint.magnitude - halfCubeDist_Z;

                    minDistance = halfCubeDist_Z;
                    
                }

                // If the slider is within the regular operating bounds, hide the other slider
                // if it is visible.
                if(name.Equals("ZSlider") && pointRelative.z < minDistance
                    && pointRelative.z >= -maxDistance)
                {
                    if (otherSlider.activeSelf == true)
                        otherSlider.SetActive(false);
                }

                if (name.Equals("XSlider") && pointRelative.x > minDistance
                    && pointRelative.x <= maxDistance)
                {
                    if (otherSlider.activeSelf == true)
                        otherSlider.SetActive(false);
                }


                // If the ZSlider is being used and 
                // if the mouse point is not behind or at the puzzle center OR
                // if the mouse point is too far behind the puzzle center (further than the starting point)
                // don't move the slider.
                if (name.Equals("ZSlider") && pointRelative.z >= minDistance)
                {                    
                    return;
                }                   

                // if we have hit the limit for the maxDistance, reset the sliders
                if (name.Equals("ZSlider") && pointRelative.z < -maxDistance)
                {
                    ResetSliders();
                    return;
                }

                // If the XSlider is being used and 
                // if the mouse point is not to the left or at the puzzle center OR
                // if the mouse point is too far to the right of the puzzle center
                // (further than the starting point)
                // don't move the slider.
                if (name.Equals("XSlider") && pointRelative.x <= minDistance)
                {
                    return;
                }
                // If we hit the limit for maxDistance, reset the sliders
                if(name.Equals("XSlider") && pointRelative.x > maxDistance)
                {
                    ResetSliders();
                    return;
                }

                // TODO
                // Make this distance possibly be equal to the mouse position - slider position
                // so that it doesn't get a weird offset sometimes
                //Vector3 prevPosition = transform.position;

                // Set the position of the slider to equal the point where the mouse
                // intersects the plane along the respective side of the cube.

                //Old method
                //transform.position = pointOnPlane;

                /*
                if (name.Equals("XSlider"))
                    transform.position = new Vector3(pointOnPlane.x, 0f, axisDistance);
                if (name.Equals("ZSlider"))
                    transform.position = new Vector3(axisDistance, 0f, pointOnPlane.z);
                */
                if (name.Equals("XSlider"))
                    transform.localPosition = new Vector3(pointRelative.x, 0f, axisDistance);
                if (name.Equals("ZSlider"))
                    transform.localPosition = new Vector3(axisDistance, 0f, pointRelative.z);

                //Debug.Log(transform.position);

                /*
                // Eliminate any vertical (y-axis) movement in relation to the cube.
                // This forces it to only move towards or away from the cube.
                transform.localPosition = 
                    new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
                */
            }
        }// end of if GetMouseButton(0) aka LMB is being held down

        if (Input.GetMouseButtonUp(0))
        {
            unclicked = true;

            sliding = false;
        }

        // if the current slider is active, reset the slider positions (prevents calling
        // this function on an inactive object).
        if(Input.GetKeyUp("r") && gameObject.activeSelf == true)
        {
            ResetSliders();
            return;
        }


        timeTracker += Time.deltaTime;
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
        //if (otherSlider != null && otherSlider.GetComponent<SliderScript>().sliding == true)
        //{
        if (otherSlider != null)
        {            
            //return;
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
                    ResetSliders();
                    return;
                }
            }

            // Hide the other slider as soon as the left mouse button is clicked
            if(Input.GetMouseButtonDown(0))
            {
                if (name == "XSlider" && otherSlider == null)
                    otherSlider = GameObject.Find("ZSlider");

                sliding = true;

                otherSlider.SetActive(false);
            }

            if (Input.GetMouseButtonUp(0))
            {
                unclicked = true;

                sliding = false;
            }



        }
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

            // The Z-axis distance the x slider will always remain from the center
            axisDistance = halfCubeDist_Z + 1;

            otherSlider = GameObject.Find("ZSlider");
        }
        else if (gameObject.name == "ZSlider")
        {
            // set the position of the sliders in relation to the main cube body
            transform.position = new Vector3(-halfCubeDist_X - 1, 0, -halfCubeDist_Z - 1 - colliderXDist);
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

            // The X-axis distance the z slider will always remain from the center
            axisDistance = -halfCubeDist_X - 1;

            otherSlider = GameObject.Find("XSlider");
        }


        movePlane = new Plane(axisReference2.transform.position, transform.position, axisReference.transform.position);

        // store a gameobject that is parented to the gameManager cube puzzle
        originalLocation = new GameObject { name = "OriginalLocation" };
        originalLocation.transform.position = transform.position;
        originalLocation.transform.parent = parentObject.transform;

        // record the original distance from the center of the puzzle + offset (axisReference2)
        // this wil be the maximum distance the slider can be away from the puzzle
        startingPoint = transform.position - axisReference2.transform.position;
        // might need to swap the order of this subtraction

        // Set the closest possible position to be equal to the half of the width of the sliders
        //closestDistance = colliderXDist;

        // now we can run the Update() method
        puzzleInitialized = true;

        /*
        Debug.Log(name);
        Debug.Log("Starting Magnitude is: "+ startingPoint.magnitude);

        Debug.Log(axisReference2.transform.position);
        Debug.Log(axisReference.transform.position);
        Debug.Log(originalLocation.transform.position);
        */
    }

    public void ResetSliders()
    {
        //reset the position of the slider
        transform.position = originalLocation.transform.position;
        // make all cubes visible again
        parentManager.ShowAllCubes();

        // reset the zSlider attributes in the game manager
        parentManager.zLayerToHide = -1;
        parentManager.zSliderUnitsMoved = 0;
        parentManager.zLayerTracker = 0;

        // make the other slider visible/interative again
        otherSlider.SetActive(true);

        // reset the xSlider attributes in the game manager
        parentManager.xLayerToHide = parentManager.puzzleSize_X;
        parentManager.xSliderUnitsMoved = 0;
        parentManager.xLayerTracker = 0;

        //TODO
        // Figure out where to reset the time tracker
        // Reset the time tracker so it doesn't count to infinity
        //timeTracker = 0f;
         

        
    }

    //TODO
    // When hiding and showing layers, deleted cubes are re-shown. Make sure deleted cubes stay
    // deleted while hiding/showing layers.

    /*
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointOnPlane + new Vector3(-2,0,0), 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(oldPoint + new Vector3(-2, 0, 0), 0.5f);
    }
    */

    /*
    public void OnDrawGizmos()
    {
        if (!puzzleInitialized)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(axisReference2.transform.position, originalLocation.transform.position);
        Gizmos.DrawLine(originalLocation.transform.position, axisReference.transform.position);
        Gizmos.DrawLine(axisReference.transform.position, axisReference2.transform.position);
    


        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointOnPlane, 1);
    }
    */

}
