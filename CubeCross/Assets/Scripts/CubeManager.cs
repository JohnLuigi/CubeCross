using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour {

    public int puzzleSize = 10;
    public GameObject[,] cubeArray;
	public GameObject exampleCube;

    private float pressTime;
    public float cubeKillDelay = 0.1f;
    private float rotateTime;
    public float rotateDelay = 0.2f;

    // values for rotating the whole puzzle
    private float minX = -360.0f;
    private float maxX = 360.0f;

    private float minY = -360.0f;
    private float maxY = 360.0f;

    public float sensX = 300.0f;
    public float sensY = 300.0f;

    float rotationX = 0.0f;
    float rotationY = 0.0f;

	// create the array of cubes that will make up the puzzle
	void Start () {
		
		CreateCubes ();
	}

	void Update () {
		
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

    // cast a ray at the location of the mouse click
    void CheckCube(float timePassed)
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
            if(hitPoint.collider.tag == "KeyCube")
            {
                Debug.Log("This cube is cube needs to stay.");
            }

            // if we hit a cube that is not part of the solution, "delete" the cube
            else if(hitPoint.collider.tag == "BlankCube")
            {
                // TODO
                // make an animation for deleting a blank cube
                hitPoint.transform.gameObject.SetActive(false);
                rotateTime = 0.0f;  // start timer to make the puzzle not able to rotate until enough time has passed
            }
        }


    }

    // do this at the end up each update to keep track of how long LMB has been pressed
    void UpdatePressTime()
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

    // rotate this gameObject to also affect all the cubes that are a child of it
    void RotatePuzzle()
    {
        if (rotateTime < rotateDelay)
            return;

        rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);
        transform.localEulerAngles = new Vector3(rotationY, -rotationX, 0);
    }

    // initialize the cube array
    void CreateCubes()
	{
		// set a starting point based on the number of cubes to be created
		// this starting point should be half of the number of cubes, centered at 0,0,0
		Vector3 startingPoint = new Vector3((float) ((puzzleSize / 2.0f) * -1.0f) + 0.5f, (float)((puzzleSize / 2.0f) * -1.0f) + 0.5f, 0);

		cubeArray = new GameObject[puzzleSize,puzzleSize];

		for (int i = 0; i < puzzleSize; i++)
        {			
            for(int j = 0; j < puzzleSize; j++)
            {
                GameObject newCube = Instantiate(exampleCube, startingPoint, Quaternion.identity) as GameObject;
                newCube.transform.localScale = Vector3.one;
                newCube.transform.parent = gameObject.transform;        // set each cube as a child of this game manager
                                                                        // so that you can manipulate the manager's transform
                                                                        // to manipualte all the cubes
                cubeArray[i,j] = newCube;

                startingPoint += new Vector3(1.0f, 0, 0);
            }
            // reset and update the starting position for the next row
            startingPoint += new Vector3((float)puzzleSize * -1.0f, 1.0f, 0);
			
		}
	}

    void CameraZoom()
    {
        /*
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);

        */
    }

    void CameraMove()
    {
        /*
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        */
    }
}
