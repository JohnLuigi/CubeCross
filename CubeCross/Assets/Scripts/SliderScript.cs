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

    public GameObject otherSlider;

    public int distanceInt; // This value will be read by the cube manager to determine how far it is from the puzzle
    // private int puzzSize;

    //private float halfCubeDist;
    private float halfCubeDist_X;
    private float halfCubeDist_Y;
    private float halfCubeDist_Z;

    private GameObject puzzleSelector;  // reference to the puzzle selection UI element

    private bool puzzleInitialized = false;

    // Distance trackers to see what direction the slider is moving in relation to the cube
    private float oldDist;
    private float newDist;

    private float startXDist;
    private float startYDist;
    private float startZDist;

    private bool minHit;

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

        if (name == "XSlider" && otherSlider == null)
        {
            otherSlider = GameObject.Find("ZSlider");
        }

        // As a default, the minimum distance has not been hit yet.
        minHit = false;


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
                startingPoint = transform.position - axisReference2.transform.position;
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
        

    }

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
        movePlane = new Plane(axisReference2.transform.position, transform.position, axisReference.transform.position);

        // if the left mouse button is held over the slider controller
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float newDistance;
            if (movePlane.Raycast(ray, out newDistance))
            {
                //Vector3 pointOnPlane = ray.origin + (ray.direction * newDistance);
                pointOnPlane = ray.GetPoint(newDistance);
            }

            // the first movement, where the oldPoint is undefined
            if (unclicked)
            {
                // just save the location hit, since it's the first time the slider is hit it can't move until
                // it is dragged
                oldPoint = pointOnPlane;
                unclicked = false;
            }
            else
            {

                
                // TODO
                // Make this distance possibly be equal to the mouse position - slider position
                // so that it doesn't get a weird offset sometimes
                Vector3 prevPosition = transform.position;

                // Set the position of the slider to equal the point where the mouse
                // intersects the plane along the respective side of the cube.
                transform.position = pointOnPlane;

                // Eliminate any vertical (y-axis) movement in relation to the cube.
                // This forces it to only move towards or away from the cube.
                transform.localPosition = 
                    new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);

                //CheckDistance();

                //Mathf.Clamp(transform.localPosition.x, closestDistance, startingPoint.magnitude);

                // Undo the movement of this frame if the distance is out of bounds
                //ClampSlider(prevPosition);
                    




                // clamp the movement to not go past the starting distance from the cube
                Vector3 currentDistance = transform.position - axisReference2.transform.position;

                // get the distance between the two points
                float distanceRounded = Vector3.Distance(transform.position, axisReference2.transform.position);
                distanceInt = Mathf.FloorToInt(distanceRounded);
                //Debug.Log(distanceInt);

                // TODO
                // replace currentDistance with newDistance
                // change the comparisons based on the slider (since the Z axis values are going to be 
                // dealing with negatives)

                // TODO
                // Handle the edge cases of the sliders and when they are at maximum and minimum sliding
                // positions in relation to the cube

                float curDist = Vector3.Distance(transform.position, axisReference2.transform.position);
                
                bool compFar = (curDist > startingPoint.magnitude);

                bool compClose = (curDist < closestDistance);
                
                if (compFar)
                {

                    //    transform.position = Vector3.MoveTowards(transform.position, axisReference2.transform.position,
                    //-step);


                    Debug.Log("too far");
                    //transform.position = originalLocation.transform.position;


                    if (name.Equals("XSlider"))
                    {
                        transform.position = originalLocation.transform.position;
                    }
                    else if (name.Equals("ZSlider"))
                    {
                        transform.position = originalLocation.transform.position;
                    }

                    //Debug.Log("too far");
                    //return;
                }
                else if (compClose)
                {
                    Debug.Log("too close");
                    //    transform.position = Vector3.MoveTowards(transform.position, axisReference2.transform.position,
                    //-step);
                    if(name.Equals("XSlider"))
                    {
                        transform.position = new Vector3(closestDistance, 0f, 0f);
                    }
                    else if(name.Equals("ZSlider"))
                    {
                        transform.position = new Vector3(0f, 0f, -closestDistance);
                    }
                    
                    //Debug.Log("too close");
                    //return;
                }
                
                
                // clamp the movement to not go too close to the center of the puzzle

                // save the point after translating the slider
                oldPoint = pointOnPlane;

                // Update the old distance after the slider has moved
                oldDist = Vector3.Distance(transform.position, axisReference2.transform.position);
            }
        }// end of if GetMouseButton(0) aka LMB is being held down

        if (Input.GetMouseButtonUp(0))
        {
            unclicked = true;

            sliding = false;
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

            
            
        }
    }

    /*
    private void OnMouseExit()
    {
        unclicked = true;
        sliding = false;
    }
    */

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
        startingPoint = transform.position - axisReference2.transform.position;
        // might need to swap the order of this subtraction

        // This variable will be used to compare what direction the slider moved since the last frame
        oldDist = Vector3.Distance(transform.position, axisReference2.transform.position);

        // Set the distances in each dimension from the parent object (center of the puzzle).
        // These values will be used to see if the sliders have reached the limits of their
        // distances from the puzzle center.
        startXDist = transform.localPosition.x;
        startYDist = transform.localPosition.y;
        startZDist = transform.localPosition.z;

        // Set the closest possible position to be equal to the half of the width of the sliders
        closestDistance = colliderXDist;

        // now we can run the Update() method
        puzzleInitialized = true;
    }

    // See if the slider is too close or too far from the puzzle
    // If so, set the position of the slider accordingly.
    public void CheckDistance()
    {
        // Disance of the center of the slider to the one end of its box collider
        // aka half the width of the slider itself.
        float colliderXDist = this.GetComponent<BoxCollider>().size.x / 2f;

        if (name.Equals("XSlider"))
        {
            // If the slider moved too far away from the puzzle (on the +X axis)
            // reset the position to its starting point.
            if (transform.localPosition.x > startXDist)
                transform.position = originalLocation.transform.position;

            // If the slider is too close to the center, set it to be 0 on the X Axis
            else if ((transform.localPosition.x - colliderXDist) < 0)
            {
                transform.localPosition =
                    new Vector3(0f, transform.localPosition.y, transform.localPosition.z);
            }

        } // End of XSlider block

        else if (this.name.Equals("ZSlider"))
        {
            // If the slider has moved too far away from the puzzle (-Z axis),
            // reset the position to the starting point.
            if (transform.localPosition.z < startZDist)
                transform.position = originalLocation.transform.position;
            // If the slider is too close to the center, set it to be 0 on the Z Axis
            else if ((transform.localPosition.z + colliderXDist) > 0f)
            {
                Debug.Log("hit the min distance");
                transform.position =
                    new Vector3(transform.position.x, transform.position.y, 0f);
            }
                
        }// End of ZSlider block
    }

    // Method that checks the distance of the point the mouse is at to the slider axis reference.
    // If the distance is too great or too small, don't update the slider's position. 
    public void ClampSlider(Vector3 inputPos)
    {
        /*
        // This vector is the difference between the input mouse point and the axisReference
        // that lies along the same axis as the slider.
        float distance = Vector3.Distance(transform.position, axisReference2.transform.position);
        
        // If the slider is currently too far from the reference, set the distance to the previous point
        if(distance >= startingPoint.magnitude)
            transform.position = inputPos;
        // If the slider is currently too close to the reference, set the distance to the previous point
        else if (distance <= closestDistance)
        {
            if(name.Equals("XSlider") && transform.localPosition.x > 0)
            {
                transform.position = inputPos;
            }
            
        }
        */
        if (name.Equals("XSlider") && transform.localPosition.x > 0)
        {
            transform.position = inputPos;
        }


    }


    /*
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointOnPlane + new Vector3(-2,0,0), 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(oldPoint + new Vector3(-2, 0, 0), 0.5f);
    }
    */
}
