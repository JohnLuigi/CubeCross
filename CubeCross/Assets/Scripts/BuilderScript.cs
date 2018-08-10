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

    // Arrays of 4 Vector2s that will give a certain square based on the 12x12 texture of the cubes
    public Vector2[] lightGrayFaceCoords;
    public Vector2[] darkGrayFaceCoords;
    public Vector2[] redFaceCoords;
        
    // Tracks whether this cube had its color temporarily changed or not.
    public bool colorWasChanged;

    // Reference to the cube the mouse is hovering over
    public GameObject cubeHoveringOver;


    // Name of the last face of the last cube that was hovered over by the mouse
    public string lastFace;

    // The number of cubes that has been placed
    public int cubeCount;






    void Start () {

        // the game starts at 1 cubes having been built (index 0)
        cubeCount = 0;
        lastFace = "";

        colorWasChanged = false;

        fullBuildCube = GameObject.Find("BuildCube");

        CubeFacesScript faceScript = fullBuildCube.GetComponent<CubeFacesScript>();

        float div = faceScript.div;

        // Set the faces for the fullBuildCube to be all one color
        lightGrayFaceCoords = new Vector2[]{
                new Vector2(0f * div, 1f * div),
                new Vector2(1f * div, 1f * div),
                new Vector2(0f * div, 0f * div),
                new Vector2(1f * div, 0f * div) };

        darkGrayFaceCoords = new Vector2[]{
                new Vector2(1f * div, 1f * div),
                new Vector2(2f * div, 1f * div),
                new Vector2(1f * div, 0f * div),
                new Vector2(2f * div, 0f * div) };

        redFaceCoords = new Vector2[]{
                new Vector2(7f * div, 1f * div),
                new Vector2(8f * div, 1f * div),
                new Vector2(7f * div, 0f * div),
                new Vector2(8f * div, 0f * div) };

        faceScript.SetAllVertices(lightGrayFaceCoords);

    }

    // Update is called once per frame
    void Update () {

        // Cast a ray every frame, in case we click on a cube or are hovering over a cube 
        // that needs a face lit up.

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // If the player does a single left click on a cube, make a new cube "attached" to its face
        if (Input.GetMouseButtonUp(0))
        {
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

                    // This is one unit away from the clicked cube, in the direction of
                    // the normal of the face that was clicked on.
                    newSpot = hit.transform.position + (hit.normal.normalized * 1.0f);

                    newCube = Instantiate(fullBuildCube, newSpot, hit.transform.rotation) as GameObject;

                    newCube.transform.parent = gameObject.transform;

                    // Icrement the number of cubes built by 1, and name the cube based on that number
                    cubeCount++;
                    newCube.name = "BuildCube" + cubeCount;

                    // TODO
                    // Set the Faces of the cube to be light or dark gray (alternating)


                    // TODO
                    // Store each cube that was made in a list.
                    // This list will eventually be saved as a puzzle solution.

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

        // If the player is simply hovering over a cube, highlight its face,
        // specifically the face of the first cube hit by the raycast.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.tag == "BuildCube")
            {
                // Reference to previous face in case a new face is hovered over
                string oldFace = lastFace;

                // Reference to previous cube object in case a new cube is hovered over
                GameObject oldCubeHoveringOver = cubeHoveringOver;
                
                // Reference to the cube being currently hovered over
                cubeHoveringOver = hit.transform.gameObject;

                //TODO
                // Set light or dark gray here.
                // If we are hovering over a new cube, set the old cube to be its default gray.
                if(cubeHoveringOver != oldCubeHoveringOver && oldCubeHoveringOver != null)
                    oldCubeHoveringOver.GetComponent<CubeFacesScript>().SetAllVertices(lightGrayFaceCoords);

                /* Triangle indices for each cube face
                     * front = 4,5
                     * right = 10,11
                     * top = 2,3
                     * back = 0,1
                     * left = 8,9
                     * bottom = 6,7
                     */

                int hitTriangle = hit.triangleIndex;

                string faceHit = "";

                // WEIRD CODE
                // I had to switch the faces/triangles for front/back and left/right.
                // Not sure why. Face selection was working fine until this set of if statements.
                if (hitTriangle == 0 || hitTriangle == 1)
                    faceHit = "front";
                if (hitTriangle == 2 || hitTriangle == 3)
                    faceHit = "top";
                if (hitTriangle == 4 || hitTriangle == 5)
                    faceHit = "back";
                if (hitTriangle == 6 || hitTriangle == 7)
                    faceHit = "bottom";
                if (hitTriangle == 8 || hitTriangle == 9)
                    faceHit = "right";
                if (hitTriangle == 10 || hitTriangle == 11)
                    faceHit = "left";

                lastFace = faceHit;

                // If the lastFace hovered over is the same as the new face (no change in face
                // being hovered over) don't do anything.
                // If it's a new cube but the same face (moving along a row of cubes) continue,
                // but if its the same face on the same cube, don't do anything.
                if (lastFace.Equals(oldFace) && cubeHoveringOver == oldCubeHoveringOver)
                    return;

                // If a new face is being hovered over, set the last face to be its
                // respective gray again.

                // TODO set light/dark gray faces to gray here
                if(oldCubeHoveringOver != null)
                    oldCubeHoveringOver.GetComponent<CubeFacesScript>().SetFace(oldFace, "LightGray");
                

                // Now that we know which face was hit, set that face to be red while we are over it
                CubeFacesScript hitCubeScript = hit.transform.gameObject.GetComponent<CubeFacesScript>();
                hitCubeScript.SetFace(faceHit, "Red");

                // Set the color change tracker to true since a color on some cube was altered, 
                // and thus needs to be undone if no cube is being hovered over in the future (else below).
                colorWasChanged = true;
            }
        } // end of raycast hit block

        // If the ray hasn't hit any cubes, set the faces of the last cube to be their original 
        // shade of gray, then set the checker to update textures to false so it isn't constantly
        // trying to set a texture somewhere.
        else
        {
            // set faces of lastCube to be dark or light
            // If the color of a cube was changed but we are no longer over a cube, undo the
            // color change of the last hit cube.
            if (colorWasChanged)
            {
                // TODO set this to be light or dark gray
                cubeHoveringOver.GetComponent<CubeFacesScript>().SetAllVertices(lightGrayFaceCoords);

                //Reset all tracking values so as if it was selecting a cube for the first time.
                lastFace = "";
                cubeHoveringOver = null;

                // Then make it so there are no cubes to have to change color
                colorWasChanged = false;
            }

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
