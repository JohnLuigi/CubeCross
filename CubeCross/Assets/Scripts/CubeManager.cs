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

    private float rotateTime;                   // rotation variables
    public float rotateDelay = 0.2f;

    public float rotationXSens = 200.0f;        
    public float rotationYSens = 200.0f;
    private float horizontalSpeed = 0.0f;
    private float verticalSpeed = 0.0f;

    public float minZoom;               // camera control variables
    public float maxZoom;
    public float maxDistance;
    //private float newZoom;

    private float smoothTime = 2.0f;    // time it will take to do the entire zoom
    private Vector3 velocity = Vector3.zero;    // velocity reference to be used by the smoothDamp function

    //private int maxCubeLayer;                   // this value will track the furthest layer of cubes from the center
                                                // to determine how far away the camera should be
    private Bounds puzzleBounds;

	// create the array of cubes that will make up the puzzle
	public void Start () {
        maxDistance = (float)puzzleSize;

        minZoom = (float)puzzleSize;
        maxZoom = (float)puzzleSize + 3.0f;

        //maxCubeLayer = puzzleSize - 1;      // the furthest cube will start at the highest index

        Camera.main.transform.position = new Vector3(0f, 0f, ((float)puzzleSize * -1.0f) - 1.0f);
		CreateCubes ();

        puzzleBounds = new Bounds(cubeArray[0, 0, 0].transform.position, Vector3.zero);
        UpdateBounds();
    }

	public void Update () {		

        // cube check/deletion block
        // if the player left clicks once, see if a cube is hit

        // first, get the time when the mouse is first held down
        // if the player drags around the object for over a second, don't make it try to delete a cube
        // only have it delete a cube if the player immediately clicks on a cube

        // if the mouse is being continuously held down, rotate the entire puzzle about the origin
        if(Input.GetMouseButton(0))
        {
            RotatePuzzle();
        }

        // on the first frame that the mouse button is clicked, reset the press timer
        if(Input.GetMouseButtonDown(0))
        {
            pressTime = 0.0f;
        }

        // when the mouse button is released, check if the cube can be deleted
        // (this will also check that enough time has passed)
        if(Input.GetMouseButtonUp(0))
        {
            CheckCube(pressTime);
            pressTime = 0.0f;
        }

        UpdatePressTime();

	}

    private void LateUpdate()
    {
        if (puzzleSize == 0)
            return;

        //UpdateBounds();
        CameraMove();
    }

    // cast a ray at the location of the mouse click
    private void CheckCube(float timePassed)
    {
        // if the player has not been holding down the left mouse button, continue with checking the cube
        // otherwise, they are lifting up after dragging for a while, so don't try to delete a cube
        if (timePassed > cubeKillDelay)
            return;

        RaycastHit hitPoint;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hitPoint, Mathf.Infinity))
        {
            // if we hit a cube that is part of the solution, punish the player
            if (hitPoint.collider.tag == "KeyCube")
            {
                Debug.Log("This cube is cube needs to stay.");
            }

            // if we hit a cube that is not part of the solution, "delete" the cube
            else if (hitPoint.collider.tag == "BlankCube")
            {
                // TODO
                // make an animation for deleting a blank cube
                hitPoint.transform.gameObject.SetActive(false); // hide the selected cube
                rotateTime = 0.0f;  // start timer to make the puzzle not able to rotate until enough time has passed
                UpdateBounds();
            }
        }
    }

    // do this at the end up each update to keep track of how long LMB has been pressed
    private void UpdatePressTime()
    {
        // add the amount of time for the frame to how long the player has been pressing down
        // in order to see if they have been dragging long enough to not try to delete a cube 
        // on letting go of the left mouse button        
        if (pressTime < 10.0f)
        {
            pressTime += Time.deltaTime;
            rotateTime += Time.deltaTime;
        }
        // capping out the max value of pressTime so that it can't count to infinity
        else
        {
            pressTime = 10.0f;
            rotateTime = 10.0f;
        }
    }

    // search the cube array for the object that is about to be deleted
    // TODO add a third index for 3d arrays
    private ArrayIndex FindObjectIndex(GameObject inputObject)
    {
        int index1 = 0;
        int index2 = 0;
        int index3 = 0;

        for (int i = 0; i < cubeArray.GetLength(0); i++)
        {
            for(int j = 0; j < cubeArray.GetLength(1); j++)
            {
                for(int k = 0; k < cubeArray.GetLength(2); k++)
                {
                    if (cubeArray[i, j, k] == inputObject)
                    {
                        index1 = i;
                        index2 = j;
                        index3 = k;
                    }
                }                
            }
        }

        return new ArrayIndex(index1, index2, index3);
    }

    // rotate this gameObject to also affect all the cubes that are a child of it
    private void RotatePuzzle()
    {
        if (rotateTime < rotateDelay)
            return;        

        horizontalSpeed = rotationXSens * Input.GetAxis("Mouse X");
        verticalSpeed = rotationYSens * Input.GetAxis("Mouse Y");

        // using the RotateAround method for X and Y axis rotation avoids a Gimbal lock (such as when
        // Euler angles were modified previously        
        transform.RotateAround(puzzleBounds.center, Vector3.down, horizontalSpeed * Time.deltaTime);       // horizontal rotation
        transform.RotateAround(puzzleBounds.center, Vector3.right, verticalSpeed * Time.deltaTime);        // vertical rotation
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
}

// TODO
// update this to add a third dimension for 3 dimensional arrays
// This class stores array indices to be used when checking for new and old arrays
// can create an instance by using:
//      ArrayIndex index = new ArrayIndex(first, second);
// and then access the indices via:
// index.firstIndex
// index.secondIndex
// if need be create set/get methods
public class ArrayIndex
{
    public int firstIndex;
    public int secondIndex;
    public int thirdIndex;

    // instantiate an instance of this item with public variables that can be directly accessed
    public ArrayIndex(int first, int second, int third)
    {
        firstIndex = first;
        secondIndex = second;
        thirdIndex = third;
    }
}
