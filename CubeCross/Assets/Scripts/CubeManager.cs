/*  Eric Rackear's take on a number puzzle game, but in three dimensions.
 *  Will attempt to have it work with minimal buttons, since it is intended
 *  to be available on android/iOS
 *
 *  Started January 10, 2018 
 *  
 * */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour {

    public int puzzleSize = 4;
    public GameObject[,,] cubeArray;            // this three dimensional array will contain all the cubes for the puzzle
	public GameObject exampleCube;
    public GameObject exampleCubeDark;

    private float pressTime;                    // cube deletion variables
    public float cubeKillDelay = 0.1f;
    public float rowDeletionDelay = 0.5f;       // delay until you can start deleting an entire row by holding down LMB
    public float sequentialDeletionDelay = 0.2f;    // delay until each cube in a row is deleted while holding down LMB
    private float sequentialDeletionTracker = 0.0f;
    private bool mouseDown = false;

    private float rotateTime;                   // rotation variables
    public float rotateDelay = 0.2f;

    public float rotationXSens = 200.0f;        
    public float rotationYSens = 200.0f;
    private float horizontalSpeed = 0.0f;
    private float verticalSpeed = 0.0f;

    public float minZoom;               // camera control variables
    public float maxZoom;
    public float maxDistance;
    private float smoothTime = 2.0f;    // time it will take to do the entire zoom

    private Vector3 velocity = Vector3.zero;    // velocity reference to be used by the smoothDamp function
                                                // to determine how far away the camera should be
    private Bounds puzzleBounds;    
    private RaycastHit[] hits;                       // an array that holds the cubes hit when holding LMB
    private List<GameObject> deletedCubes = new List<GameObject>();    // list of the deleted cubes, can be used to "undo" deletions
    private bool[] hideIndices;

    //public GameObject xSlider;      // The three objects that will be dragged by the player to hide layers of the puzzle
    //private Plane movePlane;
    /*
    public GameObject ySlider;
    public GameObject zSlider;
    */

    private Vector3 mainRotation;
    //private GameObject managerReference;

    // TODO
    // decide on how far away and where to place the sliders for X, Y, and Z
    // starting distance for the sliders

    //private float sliderDistance;
    //private Vector3 zSliderStart = new Vector3();

    private GameObject sliderReferenceX;
    private GameObject xSlider;

    private GameObject sliderReferenceZ;
    private GameObject zSlider;

    public float fadeAmount;
    public float threshValue = 8f;

    private bool firstRotation = false;
    //private bool hidden = false;

    //private SliderScript sliderScriptRef;

    // create the array of cubes that will make up the puzzle
    public void Start () {
        maxDistance = (float)puzzleSize;

        minZoom = (float)puzzleSize;
        maxZoom = (float)puzzleSize + 3.0f;
//************************************************************************************
        // ORIGINAL
        //Camera.main.transform.position = new Vector3(0f, 0f, ((float)puzzleSize * -1.0f) - 1.0f);
        Camera.main.transform.position = new Vector3(0f, 0f, (10f * -1.0f) - 1.0f);
//************************************************************************************

        CreateCubes();

        puzzleBounds = new Bounds(cubeArray[0, 0, 0].transform.position, Vector3.zero);
        UpdateBounds();

        // set the initial location of the x, y, and z sliders
        // and make their parents this gameObject (the game manager) so that when we rotate the puzzle, the slider move with it

        //sliderDistance = puzzleSize;

        // reference for the edge of the puzzle for the XSlider
        sliderReferenceX = new GameObject { name = "SliderReferenceX" };
        sliderReferenceX.transform.position = new Vector3(-(puzzleSize / 2f) + 0.5f, 0, (puzzleSize / 2f) - 0.5f);
        sliderReferenceX.transform.parent = this.transform;

        xSlider = GameObject.Find("XSlider");

        // reference for the edge of the puzzle for the ZSlider
        sliderReferenceZ = new GameObject { name = "SliderReferenceZ" };
        sliderReferenceZ.transform.position = new Vector3(-(puzzleSize / 2f) + 0.5f, 0, (puzzleSize / 2f) - 0.5f);
        sliderReferenceZ.transform.parent = this.transform;

        zSlider = GameObject.Find("ZSlider");

        // initialize each array to be visible
        // this will be used to track which "layers" of the puzzle should be hidden
        // TODO
        // might have to make a version of this for the YSlider
        hideIndices = new bool[puzzleSize];
        for (int i = 0; i < hideIndices.Length; i++)
        {
            hideIndices[i] = false;
        }

        // set the rotation to be at 45 degrees initially so the sliders aren't immediately in front of
        // the camera
        //transform.Rotate(0, 45, 0);

        //sliderScriptRef = xSlider.GetComponent<SliderScript>();

        /*
                xSlider.transform.position = new Vector3(puzzleSize, 0, 0);
                xSlider.transform.parent = gameObject.transform;
        */


        //movePlane = new Plane(new Vector3(0, 0, -1), xSlider.transform.position);

        //mainRotation = transform.eulerAngles;

        //managerReference = new GameObject();
        //managerReference.transform.rotation = transform.rotation;
    }

	public void Update () {		

        // cube check/deletion block
        // if the player left clicks once, see if a cube is hit

        // first, get the time when the mouse is first held down
        // if the player drags around the object for over a second, don't make it try to delete a cube
        // only have it delete a cube if the player immediately clicks on a cube



        // if the RMB is being continuously held down, rotate the entire puzzle about the average center of itself
        if(Input.GetMouseButton(1))
        {
            RotatePuzzle();
        }

        // on the first frame that the mouse button is clicked, reset the press timer
        if(Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            pressTime = 0.0f;
        }

        // while the left mouse button is held down, get a ray of all the cubes hit under the mouse
        // see which of those cubes are in the same column and row as the nearest, then delete each cube after
        // a delay and continue from this first array of ray-hit cubes until they run out or the player lets go of the
        // left mouse button
        if (Input.GetMouseButton(0))
        {            
            LongCheckCubes(pressTime);            
            //Slider();
        }

        // when the mouse button is released, check if the cube can be deleted
        // (this will also check that enough time has passed)
        if(Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            CheckCube(pressTime);
            pressTime = 0.0f;
        }

        // if the player presses Z, return the last cube that was deleted to the puzzle
        // this will mostly be used for debugging or puzzle creation
        if(Input.GetKeyUp("z"))
        {
            // make the last deleted cube visible again
            // then remove it from the list of deleted cubes
            if(deletedCubes.Count != 0)
            {
                deletedCubes[deletedCubes.Count - 1].SetActive(true);
                deletedCubes.RemoveAt(deletedCubes.Count - 1);
            }            
        }

        UpdatePressTime();

        //HideCubes(xSlider);
        //HideCubes(zSlider);

        if(xSlider.GetComponent<SliderScript>().sliding == true)
        {
            HideCubes(xSlider);
        }
        else if(zSlider.GetComponent<SliderScript>().sliding == true)
        {
            HideCubes(zSlider);
        }
        

        //Debug.Log("X: " + transform.eulerAngles.x + ". Y: " + transform.eulerAngles.y + ". Z: " + transform.eulerAngles.z);

        //Debug.Log("Manager X: " + managerReference.transform.rotation.eulerAngles.x + ". Y: " 
        //    + managerReference.transform.rotation.eulerAngles.y + ". Z: " + managerReference.transform.rotation.eulerAngles.z);

    }

    // do this on the first frame after everything has been set up
    private void LateUpdate()
    {
        if(!firstRotation)
        {
            // set the rotation to be at 45 degrees initially so the sliders aren't immediately in front of
            // the camera
            transform.Rotate(0, 45, 0);
            firstRotation = true;
        }
    }

    // TODO
    // Do this for all three sliders
    private bool CheckSliders(GameObject inputSlider)
    {
        SliderScript tempScript = inputSlider.GetComponent<SliderScript>();

        // if we are not over the slider and sliding, return true
        if (tempScript.sliding == true)
        {
            return true;
        }
        else
            return false;
    }

/*
    private void LateUpdate()
    {
        if (puzzleSize == 0)
            return;

        //UpdateBounds();

//************************************************************************************
        // REENABLE FOR NORMAL MOUSE MOVEMENT
        //CameraMove();
//************************************************************************************

        //transform.localRotation = Quaternion.Euler(mainRotation);
    }
*/
    // cast a ray at the location of the mouse click, and delete the one cube that is hit, if a cube is hit
    // this is only done when the mouse button is released, and thus can only delete one cube at a time
    private void CheckCube(float timePassed)
    {

        // if the player has not been holding down the left mouse button, continue with checking the cube
        // otherwise, they are lifting up after dragging for a while, so don't try to delete a cube
        if (timePassed > cubeKillDelay)
            return;

        // if a raycast is made, create an array of all the objects hit
        RaycastHit hit;                          // an array that holds the cubes hit when holding LMB
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.transform.gameObject.tag == "KeyCube")
            {
                Debug.Log("This cube is part of the solution and needs to stay. PUNISH");
            }
            else if(hit.transform.gameObject.tag == "BlankCube")
            {
                // if the cube is partially faded, don't delete it
                if(hit.transform.gameObject.GetComponent<Renderer>().material.color.a != 1f)
                {
                    return;
                }

                // TODO
                //make an animation for deleting a blank cube, play it here
                deletedCubes.Add(hit.transform.gameObject);
                hit.transform.gameObject.SetActive(false);
                rotateTime = 0.0f;
                UpdateBounds();
            }
        } 
    }

    // this method is used while the left mouse button is held down
    private void LongCheckCubes(float timePassed)
    {
        // get the inital array of cubes hit by the ray when the mouse was clicked
        if (mouseDown == true && timePassed >= rowDeletionDelay)
        {
            // if a raycast is made, create an array of all the objects hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray);
            // sort the array of hits to be from closest to furthest so the hits[0] index is the closest cube to the camera
            SortCubes(hits);
            PruneCubes(ref hits);

            mouseDown = false;
        }
        else if(timePassed >= rowDeletionDelay)
        {
            // only check for a closest hit cube if any cubes were hit
            if (hits.Length > 0)
            {
                /*
                // prune the array of cubes that are not to be deleted
                PruneCubes(hits);
                */

                // this is the cube being deleted, it is the closest to the camera and still in the same row/column
                GameObject closestCube = hits[0].transform.gameObject;

                // see that the mouse is still hitting this cube
                RaycastHit hit;                          
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // TODO
                    // somewhere in here delete the cubes at a delayed pace
                    // if we have confirmed that the mouse is still over the cube to be deleted, delete te cube
                    if (hit.transform.gameObject == closestCube)
                    {
                        if (sequentialDeletionTracker >= sequentialDeletionDelay)
                        {
                            // if the cube hit is part of the puzzle, punish the player for trying to delete it
                            if (closestCube.tag == "KeyCube")
                            {
                                Debug.Log("This cube is part of the solution and needs to stay. PUNISH");
                            }
                            // if the cube is not part of hte puzzle hide it
                            else if (closestCube.tag == "BlankCube")
                            {
                                // if the cube is partially faded, don't delete it
                                if (closestCube.GetComponent<Renderer>().material.color.a != 1f)
                                {
                                    return;
                                }

                                // TODO
                                //make an animation for deleting a blank cube, play it here
                                deletedCubes.Add(closestCube);

                                // remove the cube from the hit cubes array
                                List<RaycastHit> tempList = new List<RaycastHit>(hits);
                                tempList.Remove(hits[0]);
                                hits = tempList.ToArray();
                                
                                closestCube.SetActive(false);

                                sequentialDeletionTracker = 0.0f;
                                rotateTime = 0.0f;  // start timer to make the puzzle not able to rotate until enough time has passed
                                // TODO
                                // might need to reenable this line
                                //UpdateBounds();
                            }
                        }
                        // break out of the loop if the cube isn't the closest or if enough time hasn't passed until the next
                        // chance to delete cubes
                        else
                        {
                            sequentialDeletionTracker += Time.deltaTime;
                        }

                    }
                    
                }                
            }
        }
        else
            return;
    }

    // do this at the end up each update to keep track of how long LMB has been pressed
    private void UpdatePressTime()
    {
        // add the amount of time for the frame to how long the player has been pressing down
        // in order to see if they have been dragging long enough to not try to delete a cube 
        // on letting go of the left mouse button        
        if (pressTime < 20.0f)
        {
            pressTime += Time.deltaTime;
            rotateTime += Time.deltaTime;
        }
        // capping out the max value of pressTime so that it can't count to infinity
        else
        {
            pressTime = 20.0f;
            rotateTime = 20.0f;
        }
    }

    // rotate this gameObject to also affect all the cubes that are a child of it
    private void RotatePuzzle()
    {
        if (rotateTime < rotateDelay)
            return;

        horizontalSpeed = rotationXSens * Input.GetAxis("Mouse X");
        verticalSpeed = rotationYSens * Input.GetAxis("Mouse Y");

        // will use this to track where the puzzle is being rotated, since we'll need special handling around 0 rotation
        //float prevXRotation = transform.eulerAngles.x;
        //float prevZRotation = transform.eulerAngles.z;

        // using the RotateAround method for X and Y axis rotation avoids a Gimbal lock (such as when
        // Euler angles were modified previously     

        /*
                // calculate the intended vertical rotation
                // if it will be past the clamping points, do not perform the rotation
                Vector3 center = puzzleBounds.center;
                Vector3 axis = Vector3.right;
                float angle = verticalSpeed * Time.deltaTime;

                Vector3 position = transform.position;  // the position of this object
                Quaternion rotation = Quaternion.AngleAxis(angle, axis);    // get the desired rotation
                Vector3 direction = position - center;  // find the current direction relative to the center
                direction = rotation * direction;       // rotate the direction
                Vector3 newPosition = transform.position;
                newPosition =  center + direction;   // define the new position
                // rotate the object to keep looking at the center
                Quaternion newRotation = transform.rotation;
                Quaternion finalRotation = transform.rotation;
                finalRotation *= Quaternion.Inverse(newRotation) * rotation * newRotation;

                // check the final rotation and see if it is within the desired bounds
                if (prevXRotation >= 0 && prevXRotation < 90)
                {
                    if (finalRotation.eulerAngles.x > 90)
                        return;
                }
                // check if the angle is within 270 and 360
                else if (prevXRotation > 270 && prevXRotation < 360)
                {
                    if (finalRotation.eulerAngles.x < 270)
                        return;

                }
        */
        /*
                // check the z axis rotation
                if (prevZRotation >= 0 && prevZRotation < 90)
                {
                    if (finalRotation.eulerAngles.z > 90)
                        return;
                }
                // check if the angle is within 270 and 360
                else if (prevZRotation > 270 && prevZRotation < 360)
                {
                    if (finalRotation.eulerAngles.z < 270)
                        return;

                }
        */

        //OLD LINE ***************************************************
        //transform.RotateAround(puzzleBounds.center, Vector3.down, horizontalSpeed * Time.deltaTime);       // horizontal rotation

        //Vector3 dir = puzzleBounds.center - Vector3.down;
        //dir = Quaternion.Euler(horizontalSpeed * Time.deltaTime);
        //mainRotation = Quaternion.Euler(horizontalSpeed * Time.deltaTime, 0, 0) * Vector3.down;

        //managerReference.transform.RotateAround(puzzleBounds.center, Vector3.down, horizontalSpeed * Time.deltaTime);
        //managerReference.transform.RotateAround(puzzleBounds.center, Vector3.right, verticalSpeed * Time.deltaTime);

        transform.RotateAround(puzzleBounds.center, Vector3.down, horizontalSpeed * Time.deltaTime);
        transform.RotateAround(puzzleBounds.center, Vector3.right, verticalSpeed * Time.deltaTime);
        //Debug.Log("EulerX: " + transform.eulerAngles.x);

        //if (transform.eulerAngles.x > 90)
            //Debug.Log("more than 90");
/*
        transform.eulerAngles = new Vector3(ClampAngle(transform.eulerAngles.x, -90, 90), transform.eulerAngles.y,
            transform.eulerAngles.z);
*/


       // float prevX = transform.eulerAngles.x;
       // float prevY = transform.eulerAngles.y;
      //  float prevZ = transform.eulerAngles.z;

        
        // make the vertical rotation
        //transform.RotateAround(puzzleBounds.center, Vector3.right, verticalSpeed * Time.deltaTime);       // vertical rotation

        //mainRotation = Quaternion.Euler(verticalSpeed * Time.deltaTime, 0, 0) * Vector3.right;
        // check if the rotation is beyond the bounds of 90, 270, etc.

        
/*
        // check the final rotation and see if it is within the desired bounds
        if (prevX>= 0f && prevX <= 90f)
        {
            Debug.Log("lower quarter");
            if (transform.eulerAngles.x > 90f)
            {
                Debug.Log("lower limit hit");
                // can do an opposite rotation in order to cancel out the previous rotation
                //transform.RotateAround(puzzleBounds.center, Vector3.right, -1.0f * verticalSpeed * Time.deltaTime);
            }

        }
        // check if the angle is within 270 and 360
        else if (prevXRotation >= 270f && prevXRotation < 359f)
        {
            if (transform.eulerAngles.x < 273.9f)
            {
                // can do an opposite rotation in order to cancel out the previous rotation
                //transform.RotateAround(puzzleBounds.center, Vector3.right, -1.0f * verticalSpeed * Time.deltaTime);
            }

        }
*/

/*
        // check the z axis rotation
        if (prevZRotation >= 0 && prevZRotation < 90)
        {
            if (finalRotation.eulerAngles.z > 90)
                return;
        }
        // check if the angle is within 270 and 360
        else if (prevZRotation > 270 && prevZRotation < 360)
        {
            if (finalRotation.eulerAngles.z < 270)
                return;

        }

          

        /*
                // the desired vertical angle will be verticalSpeed * Time.deltaTime
                Vector3 desiredRot = new Vector3(transform.eulerAngles.x + (verticalSpeed * Time.deltaTime),
                    transform.eulerAngles.y, transform.eulerAngles.z + (verticalSpeed * Time.deltaTime));

                // see if the angle will be within bounds
                // first check whether the previous angle was within 0 and 90
                if(prevXRotation >= 0 && prevXRotation < 90)
                {
                    if (desiredRot.x > 90)
                        desiredRot = new Vector3(89.9f, transform.eulerAngles.y, transform.eulerAngles.z);
                }
                // check if the angle is within 270 and 360
                else if(prevXRotation > 270 && prevXRotation < 360)
                {
                    if (desiredRot.x < 270)
                        desiredRot = new Vector3(270.1f, transform.eulerAngles.y, transform.eulerAngles.z);

                }

                // check the z axis rotation
                if(prevZRotation >= 0 && prevZRotation < 90)
                {
                    if (desiredRot.z > 90)
                        desiredRot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 89.9f);
                }
                // check if the angle is within 270 and 360
                else if (prevZRotation > 270 && prevZRotation < 360)
                {
                    if (desiredRot.z < 270)
                        desiredRot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 270.1f);

                }

                transform.rotation = Quaternion.Euler(desiredRot);
        */
        //else
        //{

        //}

        // TODO
        // update the look rotation so that the vertical rotation is always relative to the camera

    }

    // initialize the cube array
    private void CreateCubes()
	{
		// set a starting point based on the number of cubes to be created
		// this starting point should be half of the number of cubes, centered at 0.5 units from there due to the cubes being 1 unit
		Vector3 startingPoint = new Vector3((float) ((puzzleSize / 2.0f) * -1.0f) + 0.5f, (float)((puzzleSize / 2.0f) * -1.0f) + 0.5f,
            (float)((puzzleSize / 2.0f) * -1.0f) + 0.5f);

        // TODO
        // Update this so that the dimensions of the puzzle aren't always a cube, sometimes it might be rectangular
		cubeArray = new GameObject[puzzleSize, puzzleSize, puzzleSize];

		for (int i = 0; i < cubeArray.GetLength(0); i++)
        {			
            for(int j = 0; j < cubeArray.GetLength(1); j++)
            {
                for(int k = 0; k < cubeArray.GetLength(2); k++)
                {
                    GameObject newCube;
                    // we are going to alternate between dark and light cubes based on whether the current
                    // index is even or odd
                    if(k % 2 == 0)
                    {
                        if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                        {
                            newCube = Instantiate(exampleCubeDark, startingPoint, Quaternion.identity) as GameObject;
                        }
                        else
                        {
                            newCube = Instantiate(exampleCube, startingPoint, Quaternion.identity) as GameObject;
                        }
                    }
                    else
                    {
                        if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                        {
                            newCube = Instantiate(exampleCube, startingPoint, Quaternion.identity) as GameObject;
                        }
                        else
                        {
                            newCube = Instantiate(exampleCubeDark, startingPoint, Quaternion.identity) as GameObject;
                        }
                    }
                    

                    newCube.transform.localScale = Vector3.one;
                    newCube.transform.parent = gameObject.transform;        // set each cube as a child of this game manager
                                                                            // so that you can manipulate the manager's transform
                                                                            // to manipualte all the cubes
                    newCube.GetComponent<CubeScript>().index1 = i;
                    newCube.GetComponent<CubeScript>().index2 = j;
                    newCube.GetComponent<CubeScript>().index3 = k;

                    cubeArray[i, j, k] = newCube;

                    startingPoint += new Vector3(1.0f, 0, 0);
                }
                // reset and update the starting position for the next row
                startingPoint += new Vector3((float)puzzleSize * -1.0f, 1.0f, 0);
            }
            // reset and update the starting position for the next grid
            startingPoint += new Vector3(0, (float)puzzleSize * -1.0f, 1.0f);
        }
        
    }

    private void UpdateBounds()
    {
        //TODO 
        // redo this find max two cubes method
        // it is very inefficient and performance expensive
        puzzleBounds.center = Vector3.zero;
        puzzleBounds.size = Vector3.zero;

        // you can get the length of a specific array dimension with "nameofArray".GetLength("dimension")
        for (int i = 0; i < cubeArray.GetLength(0); i++)
        {
            for(int j = 0; j < cubeArray.GetLength(1); j++)
            {
                for (int k = 0; k < cubeArray.GetLength(2); k++)
                {
                    // only factor in cubes that are active when finding the max distance
                    if (cubeArray[i, j, k].activeSelf)
                    {
                        puzzleBounds.Encapsulate(cubeArray[i, j, k].transform.position);
                    }
                }                    
            }
        }
    }

    // this moves the camera to keep it centered on the puzzle and in view, even while spinning it around
    private void CameraMove()
    {
        // get the largest dimension of the puzzle
        float[] boundsDimensions = { puzzleBounds.size.x, puzzleBounds.size.y, puzzleBounds.size.z };
        float zValue = Mathf.Max(boundsDimensions);

        //TODO
        //tweak this offset so that it isn't always at the maximum
        // maybe check the rotation of the parent (gamemanager object) and see if it is facing in such a way that
        // won't require the camera to be extremely far away        
        float newZ = ((-3.0f / 2.0f) * zValue) - 1.5f;
        Vector3 offset = new Vector3(0,0, newZ);
        //Vector3 offset = new Vector3(0, 0, (-1.0f * zValue) - 1.0f);

        // set the target for the camera to be at the longest distance away from the puzzle
        // TODO
        // update this to be less than the maximum distance when the object is long but not necessarily unable to fit onscreen
        // at the moment if the object is rectangular, it will stay on screen when it is on its side, but
        // if the short dimension is facing the camera, it is too far zoomed out to be able to comfortably click on cubes
        Vector3 newPosition = puzzleBounds.center + offset;

        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, newPosition, ref velocity, smoothTime);        
    }

    // selection sort to make sure the list of objects hit are ordered from closest to furthest
    private void SortCubes(RaycastHit[] hits)
    {
        RaycastHit tempHit;

        for(int i = 0; i < hits.Length - 1; i++)
        {
            for(int j = i + 1; j < hits.Length; j++)
            {
                if (hits[j].distance < hits[i].distance)
                {
                    tempHit = hits[j];
                    hits[j] = hits[i];
                    hits[i] = tempHit;
                    j++;
                }
            }
        }
    }

    // this will remove the cubes that are not in the same row and column as the first cube
    private void PruneCubes(ref RaycastHit[] hitArray)
    {
        if (hitArray.Length >= 2)
        {
            // if the object hit is not a cube, don't do anything
            if (!hitArray[0].transform.gameObject.GetComponent<CubeScript>())
            {
                return;
            }

            GameObject firstCube = hitArray[0].transform.gameObject;
            GameObject testCube = hitArray[1].transform.gameObject;

            // if the object being compared to is not a cube, break out of the method
            if (testCube.GetComponent<CubeScript>() == false)
            {
                return;
            }

            // depending on whether the second closest cube has the same i, j, or k, corrdinates, 
            // see if the rest of the coordinates of the other ray-hit cubes have the same pair of similar coordinates
            int checkType = 0;  // this will be the type of comparison being made, as in comparing i and j,
                                // j and k, or i and k
            if(firstCube.GetComponent<CubeScript>().index1 == testCube.GetComponent<CubeScript>().index1
                && firstCube.GetComponent<CubeScript>().index2 == testCube.GetComponent<CubeScript>().index2)
            {
                checkType = 1;
            }
            else if (firstCube.GetComponent<CubeScript>().index1 == testCube.GetComponent<CubeScript>().index1
                && firstCube.GetComponent<CubeScript>().index3 == testCube.GetComponent<CubeScript>().index3)
            {
                checkType = 2;
            }
            else if (firstCube.GetComponent<CubeScript>().index2 == testCube.GetComponent<CubeScript>().index2
                 && firstCube.GetComponent<CubeScript>().index3 == testCube.GetComponent<CubeScript>().index3)
            {
                checkType = 3;
            }
            
            // now that we have the two coordinates that the cubes have in common, make sure the rest of the cubes
            // in the initially hit array all have the same two axes as the first
            
            for(int i = 2; i < hitArray.Length; i++)
            {
                testCube = hitArray[i].transform.gameObject;

                if(testCube.GetComponent<CubeScript>() == false)
                {
                    return;
                }

                // TODO
                // There may be issues here in the future when determining if all the cubes are in the same row
                // these are the three possible cases where the cubes are in the same row or column as the first cube

                // TODO
                // refactor and clean this code up
                
                if(checkType == 1)
                {
                    if((firstCube.GetComponent<CubeScript>().index1 != testCube.GetComponent<CubeScript>().index1
                    || firstCube.GetComponent<CubeScript>().index2 != testCube.GetComponent<CubeScript>().index2))
                    {
                        // ugly way to remove the cube from the array of hit objects
                        // in the future store the hit objects from the array in a list
                        List<RaycastHit> tempList = new List<RaycastHit>(hitArray);
                        tempList.Remove(hitArray[i]);
                        hits = tempList.ToArray();
                    }
                }

                else if(checkType == 2)
                {
                    if (firstCube.GetComponent<CubeScript>().index1 != testCube.GetComponent<CubeScript>().index1
                    || firstCube.GetComponent<CubeScript>().index3 != testCube.GetComponent<CubeScript>().index3)
                    {
                        // ugly way to remove the cube from the array of hit objects
                        // in the future store the hit objects from the array in a list
                        List<RaycastHit> tempList = new List<RaycastHit>(hitArray);
                        tempList.Remove(hitArray[i]);
                        hits = tempList.ToArray();
                    }
                }

                else if(checkType == 3)
                {
                    if (firstCube.GetComponent<CubeScript>().index2 != testCube.GetComponent<CubeScript>().index2
                 || firstCube.GetComponent<CubeScript>().index3 != testCube.GetComponent<CubeScript>().index3)
                    {
                        // ugly way to remove the cube from the array of hit objects
                        // in the future store the hit objects from the array in a list                        
                        List<RaycastHit> tempList = new List<RaycastHit>(hitArray);
                        tempList.Remove(hitArray[i]);
                        hits = tempList.ToArray();

                    }
                }
            }
            
            

        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        // if an angle is in the critical region
        if(angle < 90 || angle > 270)
        {
            // convert all angles to -180 to 180
            if (angle > 180)
                angle -= 360;

            if (max > 180)
                max -= 360;

            if (min > 180)
                min -= 360;
        }
        
        // clamp the angle with the updated (if necessary) mins and maxes
        angle = Mathf.Clamp(angle, min, max);

        // if the angle is negative, convert it to be within 0 to 360
        if (angle < 0)
            angle += 360;

        return angle;
    }

    // Input a slider and see if its position is at a certain distance from the center of the puzzle.
    // At specific distances, hide the corresponding 
    private void HideCubes(GameObject inSlider)
    {
        // the furthest point from the slider is half the size of the puzzle, minus 0.5 (due to width of the cubes)
        // in the opposide direction of the center

        Vector3 sliderFromEdgeVector = Vector3.zero;
        // TODO 
        // udpate this to reflect the needed positions for each differnt slider
        if (inSlider.name == "XSlider")
        {
            sliderFromEdgeVector = inSlider.transform.position - sliderReferenceX.transform.position;
        }
        else if (inSlider.name == "ZSlider")
        {
            sliderFromEdgeVector = inSlider.transform.position - sliderReferenceZ.transform.position;
        }
        //else
            //sliderFromEdgeVector = Vector3.zero;


        // if the slider is a certain distance from the edge reference, hide cubes as necessary
        /*
        int puzzleRef = puzzleSize / 2;
        for (float i = -puzzleRef + 0.5f; i <= puzzleRef - 0.5f; i++) ;
        {

        }

        */

        //Material tempMat;
        Color tempColor;

        // the hiding threshold will be some number of units away from the puzzleSize
        // since the reference + puzzleSize is the entire width of the puzzle, so PuzzleSize number of 
        // checks need to be made

        // hide threshold is temporarily 8f;
        float hideThreshold = 8f;
        if (inSlider.name == "ZSlider")
        {
            //hideThreshold = 8f;
        }
        fadeAmount = 0.1f;
        //int tempIndex = puzzleSize - 1;

        bool magCompare = (sliderFromEdgeVector.magnitude <= hideThreshold);
        if (inSlider.name == "ZSlider")
        {
            //magCompare = (sliderFromEdgeVector.magnitude >= hideThreshold);
        }

        if (magCompare)
        {

            //Debug.Log(inSlider.name + " is under the threshold");

            int index = Mathf.FloorToInt(sliderFromEdgeVector.magnitude - puzzleSize);

            //Debug.Log("at index " + index);

            // change the values of the hideIndices less than the index to be 

            // if the index is within the bounds that are equal to the puzzle array indices
            // for examply is the slider is 4 units away from the active sliding range
            if (inSlider.name == "XSlider")
            {
                if (index <= puzzleSize - 1)
                {
                    for (int i = 0; i < hideIndices.Length; i++)
                    {

                        if (i < index)
                        {
                            hideIndices[i] = false;
                        }
                        else
                        {
                            hideIndices[i] = true;
                        }
                    }

                }
            }

            // "reverse" the values of the index since the ZSlider will want to work from layers 0 to 4
            // instead of from layers 4 to zero as the XSlider does
            else if (inSlider.name == "ZSlider")
            {
                if (index <= puzzleSize - 1)
                {
                    index -= (puzzleSize - 1);
                    index = Mathf.Abs(index);

                    for (int i = 0; i < hideIndices.Length; i++)
                    {
                        if (i > index)
                        {
                            hideIndices[i] = false;
                        }
                        else
                        {
                            hideIndices[i] = true;
                        }
                    }

                }

            }

            // if we are further than the threshold, make the final index of the layers to hide false
            if (sliderFromEdgeVector.magnitude > hideThreshold)
            {
                for (int i = 0; i < hideIndices.Length; i++)
                {
                    hideIndices[i] = false;
                }

            }

            // now that each layer has been determined whether it should be shown or not, update layers as necessary
            for (int layer = 0; layer < hideIndices.Length; layer++)
            {
                // TODO
                // change the index of layer according to the X or Z or Y slider
                // if the layer has to be hidden

                // TODO
                // this might only work if the dimensions of the puzzle are always equal, rectangular
                // puzzles might need tweaking here
                if (hideIndices[layer] == true)
                {
                    for (int i = 0; i < cubeArray.GetLength(0); i++)
                    {
                        for (int j = 0; j < cubeArray.GetLength(1); j++)
                        {
                            if (inSlider.name == "XSlider")
                            {
                                tempColor = cubeArray[i, j, layer].GetComponent<Renderer>().material.color;
                                tempColor.a = fadeAmount;

                                cubeArray[i, j, layer].GetComponent<Renderer>().material.color = tempColor;
                            }
                            else if (inSlider.name == "ZSlider")
                            {
                                tempColor = cubeArray[layer, i, j].GetComponent<Renderer>().material.color;
                                tempColor.a = fadeAmount;

                                cubeArray[layer, i, j].GetComponent<Renderer>().material.color = tempColor;
                            }

                        }
                    }
                }

                // if the layer has to be shown
                else if (hideIndices[layer] == false)
                {
                    for (int i = 0; i < cubeArray.GetLength(0); i++)
                    {
                        for (int j = 0; j < cubeArray.GetLength(1); j++)
                        {
                            if (inSlider.name == "XSlider")
                            {
                                tempColor = cubeArray[i, j, layer].GetComponent<Renderer>().material.color;
                                tempColor.a = 1f;

                                cubeArray[i, j, layer].GetComponent<Renderer>().material.color = tempColor;
                            }
                            else if (inSlider.name == "ZSlider")
                            {
                                tempColor = cubeArray[layer, i, j].GetComponent<Renderer>().material.color;
                                tempColor.a = 1f;

                                cubeArray[layer, i, j].GetComponent<Renderer>().material.color = tempColor;
                            }

                        }
                    }
                }
            }

        }

    }

    // public method to make all the cubes visible again
    public void ShowAllCubes()
    {
        for(int i = 0; i < hideIndices.Length;i++)
        {
            hideIndices[i] = false;
        }

        Color tempColor;

        for (int i = 0; i < cubeArray.GetLength(0); i++)
        {
            for (int j = 0; j < cubeArray.GetLength(1); j++)
            {
                for (int k = 0; k < cubeArray.GetLength(2); k++)
                {
                    tempColor = cubeArray[i, j, k].GetComponent<Renderer>().material.color;
                    tempColor.a = 1f;

                    cubeArray[i, j, k].GetComponent<Renderer>().material.color = tempColor;
                }
            }
        }
    }

}

//TODO
// Use this as an example for changing the material of the cubes to something that can be transparent
/*
 * 
 * public class ExampleClass : MonoBehaviour
{
    void Update()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Renderer rend = hit.transform.GetComponent<Renderer>();

            if (rend)
            {
                // Change the material of all hit colliders
                // to use a transparent shader.
                rend.material.shader = Shader.Find("Transparent/Diffuse");
                Color tempColor = rend.material.color;
                tempColor.a = 0.3F;
                rend.material.color = tempColor;
            }
        }
    }
}
 * 
 * 
 * 
 */
