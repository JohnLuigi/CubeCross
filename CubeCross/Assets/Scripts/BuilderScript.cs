using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderScript : MonoBehaviour {


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

    private float cameraDistanceMax = -50f;     // variables to be used with the scroll wheel to zoom the camera
    private float cameraDistanceMin = -2f;
    private float cameraDistance = -11f;
    private float scrollSpeed = 4.0f;

    public GameObject fullBuildCube;
    private Vector3 newSpot;        // Vector3 that will be based on the cube that was clicked to set a new cube's spot
    private GameObject newCube;     // newCube that will be instantiated as the player adds more cubes to the puzzle
    //private Vector3 startingPoint = new Vector3(0f, 0f, 0f);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // If the player does a single right click, see what face of the cube was clicked on
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject.tag == "BuildCube")
                {
                    /* Triangle indices for each cube face
                     * front = 4,5
                     * right = 10,11
                     * top = 2,3
                     * back = 0,1
                     * left = 8,9
                     * bottom = 6,7
                     */

                    // if the front was clicked
                    
                    //Quaternion buildRotation = Quaternion.Euler(hit.normal.x, hit.normal.y, hit.normal.z);

                    newSpot = hit.transform.position + (hit.normal.normalized * 1.0f);

                    newCube = Instantiate(fullBuildCube, newSpot, hit.transform.rotation) as GameObject;

                    newCube.transform.parent = gameObject.transform;

                    // way to set the rotation of an object
                    //Quaternion.LookRotation(-hit.normal.normalized)
                    
                }
            }
        }

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
    }

    // rotate this gameObject to also affect all the cubes that are a child of it
    private void RotatePuzzle()
    {
        // If enough time has not passed since an action was taken, do not attempt to rotate the puzzle.
        //if (rotateTime < rotateDelay)
            //return;

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
}
