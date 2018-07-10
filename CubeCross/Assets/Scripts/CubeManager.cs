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
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;

public class CubeManager : MonoBehaviour {

    public int puzzleSize = 4;
    public GameObject[,,] cubeArray;            // this three dimensional array will contain all the cubes for the puzzle
	public GameObject exampleCube_1;
    public GameObject exampleCubeDark_1;
    public GameObject exampleCube_0;
    public GameObject exampleCubeDark_0;
    public GameObject fullCube;

    private float pressTime;                    // cube deletion variables
    public float cubeKillDelay = 0.1f;
    public float rowDeletionDelay = 0.5f;       // delay until you can start deleting an entire row by holding down LMB
    public float sequentialDeletionDelay = 0.2f;    // delay until each cube in a row is deleted while holding down LMB
    private float sequentialDeletionTracker = 0.0f;
    private bool mouseDown = false;

    private float rotateTime;                   // rotation variables
    public float rotateDelay = 0.2f;
    private float rotSpeed = 6.0f;              // This value will be multiplied by the amount the mouse moved
                                                // to determine how many degrees the puzzle will be rotated.
    private float rotationZ = 0f;               // This will track how many degrees the puzzle has been rotated from 
                                                // its starting rotation of 0 degrees.
    private float horizontalRot;                // This will be the number of degrees to rotate horizontally in a cycle.
    private float verticalRot;                  // This will be the number of degrees to rotate vertically in a cycle.
    private Vector3 vertRotAxis;        // This custom axis will always be aligned to point to the right of the camera's
                                        // point of view, the puzzle being rotated along it towards/away from the camera.

    public float minZoom;               // camera control variables
    public float maxZoom;
    public float maxDistance;
    private float smoothTime = 2.0f;    // time it will take to do the entire zoom

    private float cameraDistanceMax = -50f;     // variables to be used with the scroll wheel to zoom the camera
    private float cameraDistanceMin = -2f;
    private float cameraDistance = -11f;
    private float scrollSpeed = 4.0f;


    private Vector3 velocity = Vector3.zero;    // velocity reference to be used by the smoothDamp function
                                                // to determine how far away the camera should be
    private Bounds puzzleBounds;    
    private RaycastHit[] hits;                       // an array that holds the cubes hit when holding LMB
    private List<GameObject> deletedCubes = new List<GameObject>();    // list of the deleted cubes, can be used to "undo" deletions
    private bool[] hideIndices;
    private bool[] hideIndices_X;   // array that will be used to track which cubes to hide along the X-axis
    private bool[] hideIndices_Z;   // array that will be used to track which cubes to hide along the Z-axis

    // The thresholds to start hiding the puzzle are one unit away from the edge, and the sliders
    // themselves are set to be at 2 units away from the edge of the puzzle
    public float hideThreshold_X;
    public float hideThreshold_Z;

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

    public int puzzleSize_X;
    public int puzzleSize_Y;
    public int puzzleSize_Z;

    private Text warnText;
    //private bool canProceed = false;    // used to check if the input puzzle file worked, then display the puzzle

    private int[,,] puzzleContentArray;

    //private string solution;

    private Text flagStatusObject;
    public bool flagStatus = false;     // this variable tracks whether the player is flagging cubes or deleting cubes
    public string tempFlagText = "Deleting";

    public GameObject puzzleSelector;
    private bool puzzleInitialized = false; // start as false until the puzzle has actually been created
    public bool buildingPuzzle = false;     // determines if a puzzle is being built, if so, don't engage in the normal actions

    //private bool hidden = false;

    //private SliderScript sliderScriptRef;

    // Stuff to do before Start()
    // Mostly used to set values that other scripts will need to access
    public void Awake()        
    {
        puzzleSelector = GameObject.Find("PuzzleSelector");

        // string to use for selecting a puzzle
        //solution = "XSolution.txt";
        //solution = "6x6x6_Solution.txt";
        //solution = "2x4x2_Solution.txt";
        //solution = "2x4x1_Solution.txt";

        warnText = GameObject.Find("WarnText").GetComponent<Text>();
        // initially don't have any text showing
        warnText.text = "";

        // add the puzzle reader/creation stuff here
        /*
        if(GetPuzzleInfo("Assets/StreamingAssets/" + solution))
        {
            // let the game carry out
            canProceed = true;
        }
        else
        {
            // if there was a problem with the reading of the puzzleFile, let the player know
            warnText.text = "There was an issue with\n your puzzle's format";
        }

        hideThreshold_X = puzzleSize_X + 1;
        hideThreshold_Z = puzzleSize_Z + 1;
        */

        // Set references to the X and Z sliders before they deactivate themselves so
        // that they can be reactivated.
        xSlider = GameObject.Find("XSlider");
        zSlider = GameObject.Find("ZSlider");
    }

    // create the array of cubes that will make up the puzzle
    public void Start()
    {
        maxDistance = (float)puzzleSize;

        minZoom = (float)puzzleSize;
        maxZoom = (float)puzzleSize + 3.0f;
        //************************************************************************************
        // ORIGINAL
        //Camera.main.transform.position = new Vector3(0f, 0f, ((float)puzzleSize * -1.0f) - 1.0f);
        Camera.main.transform.position = new Vector3(0f, 0f, (10f * -1.0f) - 1.0f);
        //************************************************************************************

        //prevMouseX = Input.mousePosition.x;
        //prevMouseY = Input.mousePosition.y;

        /*
        if (canProceed)
        {
            CreateCubes();

            puzzleBounds = new Bounds(cubeArray[0, 0, 0].transform.position, Vector3.zero);
            UpdateBounds();

            //create the puzzle itself
            //CreatePuzzle("Assets/Puzzles/XSolution.txt");
        }
        // set the initial location of the x, y, and z sliders
        // and make their parents this gameObject (the game manager) so that when we rotate the puzzle, the slider move with it

        //sliderDistance = puzzleSize;

        // reference for the edge of the puzzle for the XSlider
        sliderReferenceX = new GameObject { name = "SliderReferenceX" };
        sliderReferenceX.transform.position = new Vector3(-(puzzleSize_X / 2f), 0, (puzzleSize_Z / 2f) + 1f);
        sliderReferenceX.transform.parent = this.transform;

        xSlider = GameObject.Find("XSlider");

        // reference for the edge of the puzzle for the ZSlider
        sliderReferenceZ = new GameObject { name = "SliderReferenceZ" };
        sliderReferenceZ.transform.position = new Vector3(-(puzzleSize_X / 2f) - 1, 0, (puzzleSize_Z / 2f));
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

        // initially set the x values all to false, meaning they are not to be hidden
        hideIndices_X = new bool[puzzleSize_X];
        for (int i = 0; i < hideIndices_X.Length; i++)
            hideIndices_X[i] = false;

        // initially set the z values all to false, meaning they are not to be hidden
        hideIndices_Z = new bool[puzzleSize_Z];
        for (int i = 0; i < hideIndices_Z.Length; i++)
            hideIndices_Z[i] = false;

        // set the reference to the flag status text object
        flagStatusObject = GameObject.Find("FlagStatusText").GetComponent<Text>();
        flagStatusObject.text = tempFlagText;
        */

        //////////////////////////////////////
        /// END OF ORIGINAL START BLOCK///////
        //////////////////////////////////////





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

        // show/hide the menu when the escape key is pressed
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // TODO
            // move the menu, slide it, hide, whatever, do that here
            puzzleSelector.SetActive(!puzzleSelector.activeSelf);
        }

        // if the puzzle selection menu is active, don't make the game interactive
        if (puzzleSelector.activeSelf)
        {
            return;
        }

        // don't do anything in Update() until the puzzle has been initialized
        if (!puzzleInitialized)
            return;
            
        // cube check/deletion block
        // if the player left clicks once, see if a cube is hit

        // first, get the time when the mouse is first held down
        // if the player drags around the object for over a second, don't make it try to delete a cube
        // only have it delete a cube if the player immediately clicks on a cube



        // if the RMB is being continuously held down, rotate the entire puzzle about the average center of itself
        if (Input.GetMouseButton(1))
        {
            RotatePuzzle();
        }

        // Camera zoom block
        // update cameraDistance based on how the scroll wheel is being used
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMax, cameraDistanceMin);

        // set the camera's z value based on the updated cameraDistance
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y,
            cameraDistance);



        // on the first frame that the mouse button is clicked, reset the press timer
        if (Input.GetMouseButtonDown(0))
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
            // if we aren't set to flagging status, try to delete a cube on LMB click
            if (!flagStatus)
                LongCheckCubes(pressTime);            
            //Slider();
        }

        // when the mouse button is released, check if the cube can be deleted
        // (this will also check that enough time has passed)
        if(Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            // if we aren't set to flagging status, try to delete a cube on LMB click
            if (!flagStatus)
                CheckCube(pressTime);
            else
                FlagCube();
            pressTime = 0.0f;
        }

        // if the player presses Z, return the last cube that was deleted to the puzzle
        // this will mostly be used for debugging or puzzle creation
        if(Input.GetKeyUp("z"))
        {
            // make the last deleted cube visible again
            // then remove it from the list of deleted cubes
            if(deletedCubes.Count != 0 && deletedCubes[0] != null)
            {
                deletedCubes[deletedCubes.Count - 1].SetActive(true);
                deletedCubes.RemoveAt(deletedCubes.Count - 1);
            }            
        }

        // If the player presses the Space key, change the status of the flagStatus variable to its opposite
        // so it toggles between deleting cubes and flagging cubes
        if(Input.GetKeyUp(KeyCode.Space))
        {
            flagStatus = !flagStatus;

            if(flagStatus)            
                tempFlagText = "Marking";            
            else            
                tempFlagText = "Deleting";            

            flagStatusObject.text = tempFlagText;
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

        // update the previous mouse position
        //prevMouseX = Input.mousePosition.x;
        //prevMouseY = Input.mousePosition.y;

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

    // Flag a cube, changing its faces to the flagged version (at the moment making them red)
    private void FlagCube()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // if we clicked a cube, change its color
            if(hit.transform.gameObject.tag == "BlankCube" || hit.transform.gameObject.tag == "KeyCube")
            {
                CubeFacesScript faceScript = hit.transform.gameObject.GetComponent<CubeFacesScript>();

                string oldFB = faceScript.frontBack;
                string oldTB = faceScript.topBottom;
                string oldLR = faceScript.leftRight;

                string texture = "";

                // if the cube is not curently flagged, flag it by setting to its flagged texture
                if(!faceScript.flagged)
                {
                    // add the "_F" string to the faces and set them accordingly
                    texture = oldFB + "_F";
                    faceScript.SetFace("front", texture);
                    faceScript.SetFace("back", texture);
                    faceScript.frontBack = texture;

                    texture = oldTB + "_F";
                    faceScript.SetFace("top", texture);
                    faceScript.SetFace("bottom", texture);
                    faceScript.topBottom = texture;

                    texture = oldLR + "_F";
                    faceScript.SetFace("left", texture);
                    faceScript.SetFace("right", texture);
                    faceScript.leftRight = texture;

                    // swap the value of flagged
                    faceScript.flagged = !faceScript.flagged;
                }
                // if the cube is flagged, set it back to its original textures (remove the "_F" from each face)
                else
                {
                    // remove the "_F" portion of the string, aka the last two characters, then set the new faces 
                    texture = oldFB.Remove(oldFB.Length - 2);
                    faceScript.SetFace("front", texture);
                    faceScript.SetFace("back", texture);
                    faceScript.frontBack = texture;

                    texture = oldTB.Remove(oldTB.Length - 2);
                    faceScript.SetFace("top", texture);
                    faceScript.SetFace("bottom", texture);
                    faceScript.topBottom = texture;

                    texture = oldLR.Remove(oldLR.Length - 2);
                    faceScript.SetFace("left", texture);
                    faceScript.SetFace("right", texture);
                    faceScript.leftRight = texture;

                    // swap the value of flagged
                    faceScript.flagged = !faceScript.flagged;
                }
            }


        }
    }



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
                // see if the cube is flagged, if it is, do nothing
                bool thisFlag = hit.transform.gameObject.GetComponent<CubeFacesScript>().flagged;
                if (thisFlag)
                    return;
                Debug.Log("This cube is part of the solution and needs to stay. PUNISH");
            }
            else if(hit.transform.gameObject.tag == "BlankCube")
            {
                // if the cube is partially faded, don't delete it
                if(hit.transform.gameObject.GetComponent<Renderer>().material.color.a != 1f)
                {
                    return;
                }

                // see if the cube is flagged, if it is, do nothing
                bool thisFlag = hit.transform.gameObject.GetComponent<CubeFacesScript>().flagged;
                if (thisFlag)
                    return;

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
                                // see if the cube is flagged, if it is, do nothing
                                bool thisFlag = hit.transform.gameObject.GetComponent<CubeFacesScript>().flagged;
                                if (thisFlag)
                                    return;

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

                                // see if the cube is flagged, if it is, do nothing
                                bool thisFlag = hit.transform.gameObject.GetComponent<CubeFacesScript>().flagged;
                                if (thisFlag)
                                    return;

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
        // If enough time has not passed since an action was taken, do not attempt to rotate the puzzle.
        if (rotateTime < rotateDelay)
            return;
        
        // Define an axis that is centered on the transform that this script is attached to (the parent of the puzzle)
        // but pointing to the right (1,0,0) of the direction that the camera is facing.
        // This axis will always face to the right of the camera, so an object rotated along it will always turn
        // towards or away from the camera, vertically.
        vertRotAxis = transform.InverseTransformDirection(Camera.main.transform.TransformDirection(Vector3.right)).normalized;

        // Set the number of degrees the puzzle will attempt before rotating based on mouse movement        
        horizontalRot = Input.GetAxis("Mouse X") * -rotSpeed;   // rotSpeed was set to 1.0f 
        verticalRot = Input.GetAxis("Mouse Y") * rotSpeed;

        // Add the "z rotation" to the variable tracking the number of degrees the puzzle has rotated away and towards
        // the camera, having started at 0 degrees.
        rotationZ += verticalRot;

        // If the proposed change in degrees away or towards the camera is within a +90 or -90 from the starting point (0)
        // then allow the rotation to take place, otherwise, do not rotate vertically
        if (rotationZ <= 90f && rotationZ >= -90f)
        { 
            transform.Rotate(vertRotAxis, verticalRot);
        }

        // Rotate the puzzle around the Y axis horizontally, there is no clamping of this rotation.
        transform.Rotate(Vector3.up, horizontalRot);
        
    }

    // initialize the cube array
    private void CreateCubes()
	{
		// set a starting point based on the number of cubes to be created
		// this starting point should be half of the number of cubes, centered at 0.5 units from there due to the cubes being 1 unit
		Vector3 startingPoint = new Vector3((float) ((puzzleSize_X / 2.0f) * -1.0f) + 0.5f, (float)((puzzleSize_Y / 2.0f) * -1.0f) + 0.5f,
            (float)((puzzleSize_Z / 2.0f) * -1.0f) + 0.5f);

        // TODO
        // Update this so that the dimensions of the puzzle aren't always a cube, sometimes it might be rectangular
		cubeArray = new GameObject[puzzleSize_Z, puzzleSize_Y, puzzleSize_X];
        CubeFacesScript faces;


        for (int i = 0; i < cubeArray.GetLength(0); i++)
        {			
            for(int j = 0; j < cubeArray.GetLength(1); j++)
            {
                for(int k = 0; k < cubeArray.GetLength(2); k++)
                {
                    GameObject newCube;

                    // Depending on the corresponding cube in the array of 0 and 1s that make up the puzzle,
                    // create a cube at a similar index in the identically-sized cubeArray accordingly

                    // if the cube is to be a 1, aka a core part of the puzzle, instantiate it as a keyCube
                    if(puzzleContentArray[i, j, k] == 1)
                    {
                        //newCube = Instantiate(exampleCube_1, startingPoint, Quaternion.identity) as GameObject;
                        newCube = Instantiate(fullCube, startingPoint, Quaternion.identity) as GameObject;
                        newCube.tag = "KeyCube";
                        faces = newCube.GetComponent<CubeFacesScript>();
                        faces.SetAllFaces("1");
                    }
                    // if the cube is to be a 0, aka a deletable cube, instantiate it as a blankCube
                    else if(puzzleContentArray[i, j, k] == 0)
                    {
                        //newCube = Instantiate(exampleCube_0, startingPoint, Quaternion.identity) as GameObject;
                        newCube = Instantiate(fullCube, startingPoint, Quaternion.identity) as GameObject;
                        newCube.tag = "BlankCube";
                        faces = newCube.GetComponent<CubeFacesScript>();
                        faces.SetAllFaces("0");
                    }
                    else
                    {
                        newCube = null;
                    }

                    //TODO
                    // Clean this up, maybe set these two cases where a cube is shaded dark in the previous method 
                    // so that they are immediately created at the right texture instead of being reassigned

                    // we are going to alternate between dark and light cubes based on whether the current
                    // index is even or odd
                    if(k % 2 == 0)
                    {
                        if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                        {
                            //newCube.GetComponent<Renderer>().material.color = new Color(212f / 255f, 223f / 255f, 255f / 255f);
                            if (puzzleContentArray[i, j, k] == 1)
                            {
                                faces = newCube.GetComponent<CubeFacesScript>();
                                faces.SetAllFaces("1_D");
                                faces.dark = true;
                            }
                            else if (puzzleContentArray[i, j, k] == 0)
                            {
                                faces = newCube.GetComponent<CubeFacesScript>();
                                faces.SetAllFaces("0_D");
                                faces.dark = true;
                            }
                        }
                    }
                    else
                    {
                        if ((i % 2 == 0 && j % 2 == 0) || (i % 2 == 1 && j % 2 == 1))
                        {
                        }
                        else
                        {
                            //newCube.GetComponent<Renderer>().material.color = new Color(212f / 255f, 223f / 255f, 255f / 255f);
                            if (puzzleContentArray[i, j, k] == 1)
                            {
                                faces = newCube.GetComponent<CubeFacesScript>();
                                faces.SetAllFaces("1_D");
                                faces.dark = true;
                            }
                            else if (puzzleContentArray[i, j, k] == 0)
                            {
                                faces = newCube.GetComponent<CubeFacesScript>();
                                faces.SetAllFaces("0_D");
                                faces.dark = true;
                            }
                        }
                    }

                    newCube.transform.localScale = Vector3.one;
                    newCube.transform.parent = gameObject.transform;        // set each cube as a child of this game manager
                                                                            // so that you can manipulate the manager's transform
                                                                            // to manipualte all the cubes
                    newCube.GetComponent<CubeScript>().index1 = i;
                    newCube.GetComponent<CubeScript>().index2 = j;
                    newCube.GetComponent<CubeScript>().index3 = k;

                    SetFaceNumbers(newCube);

                    cubeArray[i, j, k] = newCube;

                    startingPoint += new Vector3(1.0f, 0, 0);
                }
                // reset and update the starting position for the next row
                startingPoint += new Vector3((float)puzzleSize_X * -1.0f, 1.0f, 0);
            }
            // reset and update the starting position for the next grid
            startingPoint += new Vector3(0, (float)puzzleSize_Y * -1.0f, 1.0f);
        }   // end of cube creation loop
        
    }

    // TODO
    // make this based on the first line that might contain info aobut the puzzle
    // such as the initial dimensions, etc

    // create a puzzle based on a text file that contains a solution
    private bool CreatePuzzle(string fileName)
    {
        // use try to hanfle any problems that might arisewith reading text
        try
        {
            string line;    // this will store lines as they are read from the text file

            int i_Index = 0;
            int j_Index = puzzleSize_Y - 1;

            // create a StreamReader using the location and name of the file to read, also what encoding 
            // was used when saving the file (default for now)
            StreamReader theReader = new StreamReader(fileName, Encoding.Default);

            bool firstLineRead = false;
            GameObject newCube;

            // Immediately clean up the read after this block of code.
            // Generally use the "using" statement for potentially memory-intensive processes
            // as opposed to depending on garbage collection.
            using (theReader)
            {
                //while there are lines left in the text file, read in data
                do
                {
                    line = theReader.ReadLine();
                    //Debug.Log(line);
                    //line = theReader.ReadLine();

                    if (line != null)
                    {
                        // skip the first line of text
                        if(!firstLineRead)
                        {
                            firstLineRead = true;
                        }
                        // read in the line of text
                        // if the line is a newLine, proceed to the next line
                        else if(line == "")
                        {
                            // move to the next I index
                            i_Index++;
                        }
                        // if it is 1s and 0s, store them in the cube array
                        else
                        {
                            for(int i = 0; i < line.Length; i++)
                            {
                                //TODO
                                // this is temporary, just seeing if the puzzle can even be read
//***********************************************************************************************************************
//***********************************************************************************************************************
// TEMPORARY
//***********************************************************************************************************************
//***********************************************************************************************************************
                                // if the value is 0, set the corresponding cube to be invisble
                                if (line[i].Equals('0'))
                                {
                                    //cubeArray[i_Index, j_Index, i].GetComponent<Renderer>().enabled = false;
                                    //cubeArray[i_Index, j_Index, i].SetActive(false);

                                    // set the cube as a deletable cube
                                    // TODO 
                                    // need to alternate between light and dark
                                    Vector3 location = cubeArray[i_Index, j_Index, i].transform.position;
                                    Destroy(cubeArray[i_Index, j_Index, i]);

                                    newCube = Instantiate(exampleCube_0, location, Quaternion.identity) as GameObject;

                                    //Color tempC = newCube.GetComponent<Renderer>().material.color;

                                    //tempC = new Color(212, 223, 255);

                                    newCube.GetComponent<Renderer>().material.color = new Color(212f/255f,223f/255f, 255f/255f);
                                    newCube.GetComponent<Renderer>().material.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);




                                    newCube.transform.localScale = Vector3.one;
                                    newCube.transform.parent = gameObject.transform;        // set each cube as a child of this game manager
                                                                                            // so that you can manipulate the manager's transform
                                                                                            // to manipualte all the cubes
                                    newCube.GetComponent<CubeScript>().index1 = i_Index;
                                    newCube.GetComponent<CubeScript>().index2 = j_Index;
                                    newCube.GetComponent<CubeScript>().index3 = i;

                                    cubeArray[i_Index, j_Index, i] = newCube;

                                }
                                if(line[i].Equals('1'))
                                {
                                    // set the cube as part of the puzzle that is to NOT be deleted
                                }
                            }

                            j_Index--;
                            if(j_Index < 0)
                            {
                                j_Index = puzzleSize - 1;
                            }
                        }
                    }
                }
                while (line != null);

                // the reader is done reading the text file, so close the reader and return true to broadcast success
                theReader.Close();
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("failed to read the text file" + e.Message);
            return false;
        }
        
    }

    //TODO
    // do this in an awake function, and set puzzle size to be used in the slider script
    // in slider script set puzzSize depending on size I, J, or K depending on the distance of the slider
    // and that dimension's size (especially for non-cubic puzzles)
    // TODO
    // will probably combine this with the cube creation script
    private bool GetPuzzleInfo(string fileName)
    {
        //try
        //{
            string line;    // this string will store the text of the first line

            StreamReader theReader = new StreamReader(fileName, Encoding.Default);

            int i_Index = 0;
            int j_Index = 0;
            int lineTracker = 0;

            using (theReader)
            {
                line = theReader.ReadLine();
                lineTracker++;

                // set the contents of the first line that are separated by x characters to be the
                // I, J, and K indices of the puzzle
                if (line != null)
                {
                    string[] dimensions = line.Split('x');

                    // dimensions should now be a 3 element array with 3 strings that are the i, j, k dimensions
                    // of the puzzle in the text file
                    puzzleSize_X = int.Parse(dimensions[0]);    // k dimension
                    puzzleSize_Y = int.Parse(dimensions[1]);    // j dimension
                    puzzleSize_Z = int.Parse(dimensions[2]);    // i dimension
                }


                // TODO
                // check this with non-cubic arrays to ensure the dimensions are being used properly
                puzzleContentArray = new int[puzzleSize_Z, puzzleSize_Y, puzzleSize_X];

                // start the j index at the size of the puzzle minus one since it will be stored in an array
                j_Index = puzzleSize_Y - 1;

                // check the rest of the text file to make sure it is in the right format
                do
                {                    
                    line = theReader.ReadLine();
                    lineTracker++;

                    if (line != null)
                    {
                        // if we have hit the number of rows (vertically counted on the j aka Y axis)
                        // and the next line is blank, proceed
                        // Otherwise if the next line is not blank, there are too many rows in the input layer
                        /*
                        if (j_Index == -1 && line != "")
                        {
                            Debug.Log("Too many rows in layer " + (i_Index + 1) + ".\nAt line " + lineTracker);
                            return false;
                        }
                        else if(j_Index == -1 && line =="")
                        {
                            j_Index = puzzleSize_Y - 1;
                        }
                        */
                        // if the line is a newLine, increment the i_tracker
                        if (line == "")
                        {
                            i_Index++;
                            // if there are too many newLines, aka too many layers,
                            // return false
                            if (i_Index > puzzleSize_Z - 1)
                            {
                                Debug.Log("There are more layers than indicated in first line of the file");
                                return false;
                            }
                            // if we hit a newLine but the previous layer has too few rows
                            else if(j_Index > -1)
                            {
                                Debug.Log("Too few rows in layer " + (i_Index) + ".\nAt line " + (lineTracker - 1));
                                return false;
                            }
                            // if we hit a newLine but the previous layer has too many rows
                            else if (j_Index < -1)
                            {
                                Debug.Log("Too many rows in layer " + (i_Index) + ".\nAt line " + (lineTracker - 1));
                                return false;
                            }
                            // If we hit a newLine and the previous layer had the expected amount of rows, 
                            // reset the j aka Y value tracker to be back at the top of the cube
                            else if(j_Index == -1)
                            {
                                j_Index = puzzleSize_Y - 1;
                            }

                        }                        
                        // ensure that each line is as wide as the input x size
                        else if (line.Length == puzzleSize_X && j_Index >-1)
                        {
                            // read through the line and see if it make up of only 0s or 1s
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (line[i].Equals('0') || line[i].Equals('1'))
                                {
                                    //////////////////////////////////////////////////
                                    //                   TODO                       //
                                    //        POSSIBLY MAKE THE PUZZLE HERE         //
                                    //////////////////////////////////////////////////
                                    // because these indices are the same as the ones used in the real puzzle

                                    // add the value of the cube index character to the puzzleContentArray
                                    puzzleContentArray[i_Index, j_Index, i] = (int)Char.GetNumericValue(line[i]);
                                }
                                // if any of the numbers used in the text are not 0 or 1, 
                                // return false since it is in the wrong format
                                else
                                {
                                    Debug.Log("cube values weren't 0 or 1");
                                    return false;
                                }

                            }
                            // decrement the j aka Y tracker because we've reached the bottom layer
                            j_Index--;

                        }
                        // if a line is not the length of the puzzle's k aka X dimension, return false
                        // since there is a row that has too few or too many cubes
                        else if (line.Length != puzzleSize_X)
                        {
                            Debug.Log("Wrong number of cubes in line " + lineTracker);
                            return false;
                        }
                        // if the line isn't a newLine or a proper length line of 0s and 1s, 
                        // the text file is not in the right format so return false
                        else if(j_Index <= - 1)
                        {
                            Debug.Log("Too many rows in layer " + (i_Index + 1) + ".\nAt line " + lineTracker);
                            return false;
                        }
                        else
                        {
                            Debug.Log("line " + lineTracker + " was too long");
                            return false;
                        }
                        
                        
                    }
                    // If we have reached the end of the file, check to make sure that enough layers were made
                    else if (line == null)
                    {
                        if(i_Index != puzzleSize_Z - 1)
                        {
                            Debug.Log("Not enough layers were made to match the input values on the first line");
                            return false;
                        }
                    }
                    

                }
                while (line != null);
                

                theReader.Close();
                return true;
            }
        /*   
        }
        catch (Exception e)
        {
            Debug.Log("failed to read the text file " + e.Message);
            return false;
        }
        */
        
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

        //Material tempMat;
        Color tempColor;

        // the hiding threshold will be some number of units away from the puzzleSize
        // since the reference + puzzleSize is the entire width of the puzzle, so PuzzleSize number of 
        // checks need to be made

        // TODO make a different threshold for X and Z sliders
        // hide threshold is temporarily 8f;
        //float hideThreshold = 8f;

        

        // The opacity to set the cubes to when they are "hidden." The value ranged from 0 to 1.
        fadeAmount = 0.1f;

        //int tempIndex = puzzleSize - 1;

        bool magCompare = (sliderFromEdgeVector.magnitude <= hideThreshold_X);

        if (inSlider.name == "ZSlider")
        {
            magCompare = (sliderFromEdgeVector.magnitude <= hideThreshold_Z);
        }

        // If the distance from the slider to the reference (at the back, mid corner of the cube)
        // is less than the threshold for a specific dimension, start hiding stuff
        if (magCompare)
        {
            /*
            int index = Mathf.FloorToInt(sliderFromEdgeVector.magnitude - puzzleSize_X);

            if (inSlider.name == "ZSlider")
            {
                index = Mathf.FloorToInt(sliderFromEdgeVector.magnitude - puzzleSize_Z);
            }
            */

            // Take the X position of the X slider, and round it down to the nearest integer
            int index = Mathf.FloorToInt(inSlider.transform.position.x) - 1;

            // do the same for the Z slider if need be
            if (inSlider.name == "ZSlider")
            {
                index = Mathf.CeilToInt(inSlider.transform.position.z) + 1;
                index = Math.Abs(index);
            }

            // change the values of the hideIndices less than the index to be 

            // if the index is within the bounds that are equal to the puzzle array indices
            // for examply is the slider is 4 units away from the active sliding range
            if (inSlider.name == "XSlider")
            {
                // TODO
                // might not need this check
                if (index <= puzzleSize_X)
                {
                    for (int i = 0; i < hideIndices_X.Length; i++)
                    {
                        // Set the array indices less than the current index (derived by the slider's
                        // distance fromt he reference) to be false (don't hide) and everything greater
                        // than that index is to be hidden
                        if (i <= index)
                        {
                            hideIndices_X[i] = false;
                        }
                        else
                        {
                            hideIndices_X[i] = true;
                        }
                    }

                }
            }

            // "reverse" the values of the index since the ZSlider will want to work from layers 0 to 4
            // instead of from layers 4 to zero as the XSlider does
            else if (inSlider.name == "ZSlider")
            {
                if (index <= puzzleSize_Z)
                {
                    for (int i = 0; i < hideIndices_Z.Length; i++)
                    {
                        if (i > index)
                        {
                            hideIndices_Z[i] = false;
                        }
                        else
                        {
                            hideIndices_Z[i] = true;
                        }
                    }

                }

            }

            if(inSlider.name == "XSlider")
            {
                // now that each layer has been determined whether it should be shown or not, update layers as necessary
                for (int layer = 0; layer < hideIndices_X.Length; layer++)
                {
                    // TODO
                    // change the index of layer according to the X or Z or Y slider
                    // if the layer has to be hidden

                    // TODO
                    // this might only work if the dimensions of the puzzle are always equal, rectangular
                    // puzzles might need tweaking here
                    if (hideIndices_X[layer] == true)
                    {
                        for (int i = 0; i < cubeArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < cubeArray.GetLength(1); j++)
                            {
                                tempColor = cubeArray[i, j, layer].GetComponent<Renderer>().material.color;
                                tempColor.a = fadeAmount;

                                cubeArray[i, j, layer].GetComponent<Renderer>().material.color = tempColor;
                                SetCollider(cubeArray[i, j, layer], false);


                            }
                        }
                    }

                    // if the layer has to be shown
                    else if (hideIndices_X[layer] == false)
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
                                    SetCollider(cubeArray[i, j, layer], true);
                                }

                            }
                        }
                    }
                } // end of hide/show loop
            }
            

            // TODO
            // CHANGE FOR Z
            if(inSlider.name == "ZSlider")
            {
                // now that each layer has been determined whether it should be shown or not, update layers as necessary
                for (int layer = 0; layer < hideIndices_Z.Length; layer++)
                {
                    // TODO
                    // change the index of layer according to the X or Z or Y slider
                    // if the layer has to be hidden

                    // TODO
                    // this might only work if the dimensions of the puzzle are always equal, rectangular
                    // puzzles might need tweaking here
                    if (hideIndices_Z[layer] == true)
                    {
                        for (int j = 0; j < cubeArray.GetLength(1); j++)
                        {
                            for (int k = 0; k < cubeArray.GetLength(2); k++)
                            {
                                tempColor = cubeArray[layer, j, k].GetComponent<Renderer>().material.color;
                                tempColor.a = fadeAmount;

                                cubeArray[layer, j, k].GetComponent<Renderer>().material.color = tempColor;
                                SetCollider(cubeArray[layer, j, k], false);
                            }
                        }
                    }

                    // if the layer has to be shown
                    else if (hideIndices_Z[layer] == false)
                    {
                        for (int j = 0; j < cubeArray.GetLength(1); j++)
                        {
                            for (int k = 0; k < cubeArray.GetLength(2); k++)
                            {
                                tempColor = cubeArray[layer, j, k].GetComponent<Renderer>().material.color;
                                tempColor.a = 1f;

                                cubeArray[layer, j, k].GetComponent<Renderer>().material.color = tempColor;
                                SetCollider(cubeArray[layer, j, k], true);
                            }
                        }
                    }
                } // end of hide/show loop
            }

            


        } // end of magcompare
        if (inSlider.name == "XSlider")
        {
            // if we are further than the threshold, make the final index of the layers to hide false
            if (sliderFromEdgeVector.magnitude > hideThreshold_X)
            {
                for (int i = 0; i < hideIndices_X.Length; i++)
                {
                    hideIndices_X[i] = false;
                }
            }
        }
        if (inSlider.name == "ZSlider")
        {
            // if we are further than the threshold, make the final index of the layers to hide false
            if (sliderFromEdgeVector.magnitude > hideThreshold_Z)
            {
                for (int i = 0; i < hideIndices_Z.Length; i++)
                {
                    hideIndices_Z[i] = false;
                }
            }
        }

    }

    // public method to make all the cubes visible again
    public void ShowAllCubes()
    {
        for(int i = 0; i < hideIndices_X.Length;i++)
        {
            hideIndices_X[i] = false;
        }

        for (int i = 0; i < hideIndices_Z.Length; i++)
        {
            hideIndices_Z[i] = false;
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
                    SetCollider(cubeArray[i, j, k], true);
                }
            }
        }
    }

    // this method will be called to enable or disable a collider for a cube
    public void SetCollider(GameObject theCube, bool onOff)
    {
        Collider theCollider = theCube.GetComponent<Collider>();

        if (!theCollider)
            return;

        //theCollider.enabled = onOff;
        // using the line above caused an error when destroying a game object that had its collider disabled
        // so instead I'll use this:

        if(!theCollider.gameObject.activeInHierarchy && !onOff && theCollider.enabled)
            Debug.LogWarning("Disabling inactive collider: " + GetHierarchyPath(theCollider.transform), theCollider);
        theCollider.enabled = onOff;
    }

    // This is used in helping fix the bug with the gameobjects without colliders enabled being destroyed
    // The method goes up through the cube's hierarchy to determine the path of children-parents connected to
    // the problem cube.
    public static string GetHierarchyPath(Transform transform)
    {
        if (!transform)
            return string.Empty;

        string path = transform.name;
        while(transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }

    // TODO
    // Make this set faces to hve circles or squares depending on the number
    // on connected groups of keycubes in the row.

    // This is run once for a cube after a cube has been initialized.
    // The 6 faces will be compared to the rest of the contents of the row that it is in
    // and will set the texture of the respective face accordingly
    private void SetFaceNumbers(GameObject inputObject)
    {
        CubeFacesScript faceScript = inputObject.GetComponent<CubeFacesScript>();
        CubeScript cubeScript = inputObject.GetComponent<CubeScript>();
            
        int cubeCount = 0;  // this value will store the number of keyCubes in the current row

        // Store a copy of the row, and separate it into substrings using
        // "0" as a delimiter
        string row = "";
        string[] rowArray;  // this array will contain the substrings
                            // that will be counted to determine the 
                            // number of groups of keyCubes in a row

        char[] separator = new char[] { '0' };  // the separator to be used to split row into substrings

        // Use the three indices saved in the cubeScript to determine the
        // location of the input cube in the CubeArray

        //*********************
        // FRONT AND BACK FACES
        //*********************
        // to check the front, we are going to move along the Z axis aka the i coordinate
        // using the CubeArray whose dimensions are stored as CubeArray[z, y, x]
        for (int i = 0; i < cubeArray.GetLength(0); i++)
        {
            // add the element to the row string, either a 0 or a 1 is added
            row += puzzleContentArray[i, cubeScript.index2, cubeScript.index3].ToString();

            // if a cube in the row is a keyCube, increment the cubeCount
            if (puzzleContentArray[i, cubeScript.index2, cubeScript.index3] == 1)
            {
                cubeCount++;                 
            }
                
        }

        // now that we have the number of cubes in the Z-axis row, we can set the front and back
        // faces of the cube accordingly

        // convert the number of cubes in the row to a string
        // add the correct modifier to the string if need be (dark, highlighted, circled, squared)

        string tex = cubeCount.ToString();

        // split the row string into an array stored in rowArray
        rowArray = row.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        // the size of rowArray is the number of groups that make up the keyCubes in that row

        // If there are only two groups of keyCubes in the row, add the "_C" circle modifier to the
        // format of the texture to be place on the cube
        if (rowArray.Length == 2)
        {
            tex += "_C";
        }
        // If there are more than two groups of keyCubes, ass the "_S" modifier to the 
        // format of the texture
        else if(rowArray.Length > 2)
        {
            tex += "_S";
        }

        // TODO
        // add the other face modifiers here ESPECIALLY FLAGGED
        if (faceScript.dark)
        {
            tex += "_D";
        }

        // set the front and back
        faceScript.SetFace("front", tex);
        faceScript.SetFace("back", tex);

        // save the format the two faces were set to
        faceScript.frontBack = tex;

        //*********************
        // TOP AND BOTTOM FACES
        //*********************
        cubeCount = 0;  // reset cubeCount
        row = "";       // reset the row

        for (int j = 0; j < cubeArray.GetLength(1); j++)
        {
            // add the element to the row string, either a 0 or a 1 is added
            row += puzzleContentArray[cubeScript.index1, j, cubeScript.index3].ToString();

            // if a cube in the row is a keyCube, increment the cubeCount
            if (puzzleContentArray[cubeScript.index1, j, cubeScript.index3] == 1)
                cubeCount++;
        }

        tex = cubeCount.ToString();

        // split the row string into an array stored in rowArray
        rowArray = row.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        // the size of rowArray is the number of groups that make up the keyCubes in that row

        // If there are only two groups of keyCubes in the row, add the "_C" circle modifier to the
        // format of the texture to be place on the cube
        if (rowArray.Length == 2)
        {
            tex += "_C";
        }
        // If there are more than two groups of keyCubes, ass the "_S" modifier to the 
        // format of the texture
        else if (rowArray.Length > 2)
        {
            tex += "_S";
        }

        // TODO
        // add the other face modifiers here
        if (faceScript.dark)
        {
            tex += "_D";
        }
        // set the top and bottom
        faceScript.SetFace("top", tex);
        faceScript.SetFace("bottom", tex);

        // save the format the two faces were set to
        faceScript.topBottom = tex;

        //*********************
        // LEFT AND RIGHT FACES
        //*********************
        cubeCount = 0;  // reset cubeCount
        row = "";       // reset the row

        for (int k = 0; k < cubeArray.GetLength(2); k++)
        {
            // add the element to the row string, either a 0 or a 1 is added
            row += puzzleContentArray[cubeScript.index1, cubeScript.index2, k].ToString();

            // if a cube in the row is a keyCube, increment the cubeCount
            if (puzzleContentArray[cubeScript.index1, cubeScript.index2, k] == 1)
                cubeCount++;
        }

        tex = cubeCount.ToString();

        // split the row string into an array stored in rowArray
        rowArray = row.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        // the size of rowArray is the number of groups that make up the keyCubes in that row

        // If there are only two groups of keyCubes in the row, add the "_C" circle modifier to the
        // format of the texture to be place on the cube
        if (rowArray.Length == 2)
        {
            tex += "_C";
        }
        // If there are more than two groups of keyCubes, ass the "_S" modifier to the 
        // format of the texture
        else if (rowArray.Length > 2)
        {
            tex += "_S";
        }

        // TODO
        // add the other face modifiers here
        if (faceScript.dark)
        {
            tex += "_D";
        }
        // set the top and bottom
        faceScript.SetFace("left", tex);
        faceScript.SetFace("right", tex);

        // save the format the two faces were set to
        faceScript.leftRight = tex;
    }

    // public method that can be called to create a puzzle upon clicking a puzzle button
    public void InitializePuzzle(string puzzleName)
    {
        // try to get the puzzle information, if it works, the puzzle can be made
        if (GetPuzzleInfo(Application.streamingAssetsPath + "/" + puzzleName + ".txt"))
        {
            // let the game carry out
            //canProceed = true;
        }
        else
        {
            // if there was a problem with the reading of the puzzleFile, let the player know
            warnText.text = "There was an issue with\n your puzzle's format";
        }

        hideThreshold_X = puzzleSize_X + 1;
        hideThreshold_Z = puzzleSize_Z + 1;

        // clear out the currently existing puzzle elements
        if(cubeArray != null)
        {
            // destroy previous gameobjects if another puzzle is currently existing
            foreach (GameObject go in cubeArray)
                Destroy(go);
        }

        if (deletedCubes != null)
        {
            // destroy previously stored deleted cubes
            foreach (GameObject go in deletedCubes)
                Destroy(go);
        }

        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        rotationZ = 0f;
        // This is where the puzzle is formed in-game
        CreateCubes();        
        puzzleBounds = new Bounds(cubeArray[0, 0, 0].transform.position, Vector3.zero);
        UpdateBounds();

        // reference for the edge of the puzzle for the XSlider
        sliderReferenceX = new GameObject { name = "SliderReferenceX" };
        sliderReferenceX.transform.position = new Vector3(-(puzzleSize_X / 2f), 0, (puzzleSize_Z / 2f) + 1f);
        sliderReferenceX.transform.parent = this.transform;

        xSlider.SetActive(true);
        // initialize the X Slider
        SliderScript xScript = xSlider.GetComponent<SliderScript>();
        xScript.Initialize();

        // reference for the edge of the puzzle for the ZSlider
        sliderReferenceZ = new GameObject { name = "SliderReferenceZ" };
        sliderReferenceZ.transform.position = new Vector3(-(puzzleSize_X / 2f) - 1, 0, (puzzleSize_Z / 2f));
        sliderReferenceZ.transform.parent = this.transform;

        zSlider.SetActive(true);
        // initialize the Z Slider
        SliderScript zScript = zSlider.GetComponent<SliderScript>();
        zScript.Initialize();

        // initialize each array to be visible
        // this will be used to track which "layers" of the puzzle should be hidden
        // TODO
        // might have to make a version of this for the YSlider
        hideIndices = new bool[puzzleSize];
        for (int i = 0; i < hideIndices.Length; i++)
        {
            hideIndices[i] = false;
        }

        // initially set the x values all to false, meaning they are not to be hidden
        hideIndices_X = new bool[puzzleSize_X];
        for (int i = 0; i < hideIndices_X.Length; i++)
            hideIndices_X[i] = false;

        // initially set the z values all to false, meaning they are not to be hidden
        hideIndices_Z = new bool[puzzleSize_Z];
        for (int i = 0; i < hideIndices_Z.Length; i++)
            hideIndices_Z[i] = false;

        // set the reference to the flag status text object
        flagStatusObject = GameObject.Find("FlagStatusText").GetComponent<Text>();
        flagStatusObject.text = tempFlagText;

        puzzleInitialized = true;
    }

}
