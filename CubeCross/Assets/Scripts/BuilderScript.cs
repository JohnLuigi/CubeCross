using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEditor;

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

    // This object will be set via the inspector by dragging in the buildCube Prefab.
    public GameObject originalCube;
    // Reference to the first building cube that is the seed for the puzzle.
    private GameObject fullBuildCube;
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

    // Tracker for whether we are deleting or adding cubes
    public bool deletingCubes;
    // String that holds the text to be used to indicate whether we are adding or deleting cubes
    public string tempBuildText;
    // Text object that will display whether cubes are being added or deleted 
    private Text buildStatusObject;

    public string puzzleSaveName;

    private UIManagerScript uiManager;

    public bool usingSaveUI;    // Tracker to make the puzzle non-interactive while trying to save.

    public PuzzleSolution puzzleSolution;   // The object that will track the maximum dimensions
                                            // of the in-progress solution, and the locations of the
                                            // cubes as they are added/deleted.

    public int cubeID;  // This will be a value that increases for each cube made. A cube will be
                        // assigned an ID upon creation so that specific cube can be deleted from the
                        // list of cubes stored in puzzleSolution.

    // This array will store a reference to each cube that exists in the puzzle that is being made
    // while in build mode.
    public GameObject[] cubesToSave;

    private ClueEditScript clueEditScript;

        //TODO
        //CONTINUE FROM HERE
        // Set up the object that will contain the list of cubes/dimensions of puzzle
        // and update those values as cubes are added/deleted.




    void Start ()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManagerScript>();
        clueEditScript = GameObject.Find("ClueEditManager").GetComponent<ClueEditScript>();

        // the game starts at 1 cubes having been built (index 0)
        cubeCount = 0;
        lastFace = "";

        colorWasChanged = false;

        //TODO might have to reset this every time build mode is started.
        cubeID = 0; // Initialize the cubeID that will increment every time a cube is made.

        deletingCubes = false;  // The default for build mode is to add cubes.
        tempBuildText = "Adding";    // The default for build mode is to add cubes.

        usingSaveUI = false;    // The UI in build mode starts hidden, so it is initially not
                                // being used.

        

        // Create the first cube at 0,0,0 and with 0 rotation.
        fullBuildCube = Instantiate(originalCube, Vector3.zero, Quaternion.identity) as GameObject;
        // Parent the starting cube to the manager so it can be rotated with the puzzle.
        fullBuildCube.transform.parent = gameObject.transform;
        // The first cube gets the name "BuildCube0"
        fullBuildCube.name = "BuildCube" + cubeCount;

        // Set the first cube's attached PuzzleUnit to be at 0,0,0 with ID 0.
        CubeScript firstCubeScript = fullBuildCube.gameObject.GetComponent<CubeScript>();
        firstCubeScript.SetPuzzleUnit(0, 0, 0, 0);


        // Initialize the puzzle solution that will be converted to a JSON file when saved.
        puzzleSolution = new PuzzleSolution();

        // Add the first cube to the puzzleSolution's list of PuzzleUnits.
        puzzleSolution.AddUnit(firstCubeScript.GetPuzzleUnit());

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

        // set the reference to the buildStatusText object
        buildStatusObject = GameObject.Find("FlagStatusText").GetComponent<Text>();
        buildStatusObject.text = tempBuildText;

    }

    // Update is called once per frame
    void Update () {

        // If the savingUI is being used, don't make the puzzle interactable.
        if (usingSaveUI)
            return;

        // If the clueEditor is being used, don't do anything
        // This is mostly because saving during clue edit mode will be a different
        // save function than the build saving mode.
        if (clueEditScript.editingClues)
            return;

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
                    // if we are deleting cubes
                    if(deletingCubes)
                    {
                        //TODO
                        // Add possible undo queue adding of cubes that were deleted here.

                        // If there is only one cube left (if cubeCount is 0), don't delete the cube.
                        // AKA this if statement won't be reached.
                        if(cubeCount >= 1)
                        {
                            // Delete the cube
                            Destroy(hit.transform.gameObject);
                            // Reduce the count of cubes by 1 since a cube was deleted.
                            cubeCount--;
                        }
                        

                        // TODO
                        // update puzzle bounds here

                        return;

                        
                    }
                    // if we are set to add cubes (deleteCubes == false)
                    else if(!deletingCubes)
                    {
                        // This is one unit away from the clicked cube, in the direction of
                        // the normal of the face that was clicked on.
                        newSpot = hit.transform.position + (hit.normal.normalized * 1.0f);

                        newCube = Instantiate(originalCube, newSpot, hit.transform.rotation) as GameObject;

                        newCube.transform.parent = gameObject.transform;

                        // Icrement the number of cubes built by 1, and name the cube based on that number
                        cubeCount++;
                        newCube.name = "BuildCube" + cubeCount;

                        // TODO
                        // Set the Faces of the cube to be light or dark gray (alternating)


                        // TODO
                        // Store each cube that was made in a list.
                        // This list will eventually be saved as a puzzle solution.

                        // Get the face of the cube that was hit.
                        string faceHit = GetFaceHit(hit.triangleIndex);

                        // Get the indices of the cube that was clicked on.
                        CubeScript hitCubeScript = hit.transform.gameObject.GetComponent<CubeScript>();
                        PuzzleUnit hitUnit = hitCubeScript.GetPuzzleUnit();                        

                        // Create an array of three integers that are the change in x,y,z.
                        int[] indexChange = ReturnDimensionChange(faceHit);

                        // Create indices for the new cube based on the face of the hitCube
                        // that was clicked on.
                        int newX = hitUnit.xIndex + indexChange[0];
                        int newY = hitUnit.yIndex + indexChange[1];
                        int newZ = hitUnit.zIndex + indexChange[2];

                        cubeID++;

                        //TODO
                        // Determine if I should just use this line instead.
                        // Possibly add this unit to the list
                        PuzzleUnit newUnit = new PuzzleUnit(newX, newY, newZ, cubeID);

                        // Add this newly made puzzleUnit to the puzzleSolution's list.
                        puzzleSolution.AddUnit(newUnit);

                        // Save the newly created PuzzleUnit to the newly created cube's CubeScript.
                        CubeScript newCubeScript = newCube.GetComponent<CubeScript>();
                        newCubeScript.SetPuzzleUnit(newX, newY, newZ, cubeID);


                        /*
                        Debug.Log("OldX: " + hitUnit.xIndex + "\nOldY: " + hitUnit.yIndex
                            + "\nOldZ: " + hitUnit.zIndex + "\n\nNewX: " + newX +
                            "\nNewY: " + newY + "\nNewZ: " + newZ);
                        */
                    }


                }// end of if tag == "buildingCube"
            }// end of if raycast
        }// end of if GetMouseButtonUp(0)

        // If the player presses the Space key, change the status of the deletingCubes
        //variable to its opposite so it toggles between deleting cubes and adding cubes.
        if (Input.GetKeyUp(KeyCode.Space))
        {
            deletingCubes = !deletingCubes;

            // If set to deleting cubes, make the text show deleting.
            if (deletingCubes)
                tempBuildText = "Deleting";
            // If set to not deleting cubes, make the text show adding.
            else
                tempBuildText = "Adding";

            // Update the text of the build status object.
            buildStatusObject.text = tempBuildText;
        }

        // If the s key is pressed, save a JSON file
        if(Input.GetKeyUp(KeyCode.S))
        {
            // TODO
            // Call the method that shows a text input field to name the puzzle.
            uiManager.DisplayTextInputField(true);
            usingSaveUI = true;
            uiManager.SetInputStatus(true);
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
                // If the cube that was being hovered over no longer exists (was deleted) do no more.
                if (cubeHoveringOver == null)
                    return;

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

    // Test script for making a JSON file
    public int MakePuzzle()
    {
        // Default returnValue is 0, if 0 is returned the file saved properly
        int returnValue = 0;

        // Initialize the list of the objects that will store the cube coordinates.
        // This list will equal the list that is being updated as new cubes are added/deleted
        // while in build mode.

        // Old dummy list.
        /*
        List<PuzzleUnit> puzzleUnitList = new List<PuzzleUnit>();

        // Add a few dummy puzzle cubes to try reading from the file.
        puzzleUnitList.Add( new PuzzleUnit( 4, 5, 6, 0) );

        puzzleUnitList.Add(new PuzzleUnit(7, 8, 9, 1));
        */

        // If you need to clear the list, use:
        // puzzleUnitList.Clear();

        

        //TODO
        // Find the dimensions of the puzzle by iterating through all the stored cubes and
        // finding the max and min for each dimension.
        /*
        // Create a sample puzzleSolution
        PuzzleSolution puzzleSoln = new PuzzleSolution
        {
            xDimensionMax = 7,
            xDimensionMin = 4,
            yDimensionMax = 8,
            yDimensionMin = 5,
            zDimensionMax = 9,
            zDimensionMin = 6,
            canEdit = true,
            // old dummy list here
            //puzzleUnits = puzzleUnitList
            puzzleUnits = puzzleSolution.puzzleUnits
        };
        */

        // Create a JSON file and write the newly created puzzleSolution to it

        // The standard filename in case the inputField didn't work.
        string fileName = "Default_Puzzle_Name.json";

        // If the puzzleSaveName has been set, change it to the updated puzzlesaveName
        // via the SetPuzzleName function that was called externally.
        if(!puzzleSaveName.Equals(""))
        {
            // Save the string that was entered by the user.
            fileName = puzzleSaveName + ".json";
        }
        
        // The path to the file (will be in the StreamingAssets folder)
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        
        // See if the file exists.
        // If it does, ask the player if they want to overwrite the file.        
        if(File.Exists(filePath))
        {
            // If we try to save and the yes button was clicked, overwrite the file.
            if(uiManager.yesNoValue == 1)
            {
                SavePuzzle(filePath);
                // Reset the yesNoValue so it can be set to overwrite again.
                uiManager.yesNoValue = 0;
            }
            else if(uiManager.yesNoValue == 0)
            {
                // Ask the player if they want to overwrite the file.
                //uiManager.ToggleOverwriteText();

                // Show an option to overwrite the existing file
                // Yes          |               No
                // If no, bring back the text input.
                // If yes, call the same function below
                uiManager.ToggleYesNo();

                // If a file was found with the same name, don't try to save the puzzle, but
                // instead return a different value indicating that the player needs to overwrite 
                // or cancel saving.
                returnValue = 1;
            }
            
        }
        // If the file does not exist, create a new file
        else
        {
            SavePuzzle(filePath);
        }

        /*
        // Check if a puzzleSolution with the same name already exists or not.
        FileInfo fileInfo = new FileInfo(filePath);
        */

        return returnValue;


    }

    public void SetPuzzleName(string inputString)
    {
        puzzleSaveName = inputString;
    }

    // The function that collects all the cubes in the scene and saves them for later as a
    // JSON file.
    public void SavePuzzle(string filePath)
    {

        // Create an array of all the cubes with the BuildCube tag.
        cubesToSave = GameObject.FindGameObjectsWithTag("BuildCube");

        // Get the minimum and maximum values for each dimension of the puzzle.
        int[] minMaxValues = GetMinMaxValues(cubesToSave);

        PuzzleSolution newPuzzleSolution = new PuzzleSolution(minMaxValues, true);

        // Add each puzzleUnit attached to the cubes in cubesToSave to the PuzzleSolution.
        foreach(GameObject cube in cubesToSave)
        {
            newPuzzleSolution.AddUnit(cube.GetComponent<CubeScript>().GetPuzzleUnit());
        }

        // goal make a PuzzleSolution from cubesToSave.

        // Create a string that is made from the PuzzleSolution object
        string jsonString = JsonUtility.ToJson(newPuzzleSolution, true);
        // Write the json string to the file.
        File.WriteAllText(filePath, jsonString);

        // Call the function that shows the puzzle save confirmation text on UIManager.
        uiManager.StartSavedTextTimer();

        // Refreshes the assets in the Editor so you can immediately see the new file made.
        //AssetDatabase.Refresh();

        // If the solution was saved successfully, we are no longer using the save UI
        // so the puzzle can be interacted with again.
        usingSaveUI = false;
    }

    // This gives us the face that was hit based on the input triangleIndex integer.
    public string GetFaceHit(int hitTriangle)
    {
        string faceHit = "";

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

        return faceHit;
    }

    public int[] ReturnDimensionChange(string inFace)
    {
        // The default output if no change in x, y, or z.
        int[] output = new int[] { 0, 0, 0 };

        switch(inFace)
        {
            // Front clicked on, z+1
            case "front":
                output[2] = 1;
                break;
            // Back clicked on, z-1
            case "back":
                output[2] = -1;
                break;
            // Top clicked on, y+1
            case "top":
                output[1] = 1;
                break;
            // Bottom clicked on, y-1
            case "bottom":
                output[1] = -1;
                break;
            // Left clicked on, x+1
            case "left":
                output[0] = 1;
                break;
            // Right clicked on, x-1
            case "right":
                output[0] = -1;
                break;
            default:
                break;
        }
            
        return output;
    }

    public int[] GetMinMaxValues(GameObject[] inputCubes)
    {
        // The default output if there is only 1 cube (max is 1 cube in all directions).
        // The format of the output array is
        // {minX, maxX, minY, maxY, minZ, maxZ}
        int[] output = new int[] { 0, 0, 0, 0, 0, 0 };

        foreach(GameObject cube in inputCubes)
        {
            // Compare the x, y, and z coordinate stored in each cube
            // to the default values. If greater than a max or less than a min, update the output.
            CubeScript tempScript = cube.GetComponent<CubeScript>();

            // If we are able to get a CubeScript from the puzzle, proceed.
            // Do this to avoid picking up some random object imporperly assigned a BuildCube tag.
            if(tempScript != null)
            {
                // Test the minimum/maximum X value
                int testValue = tempScript.GetPuzzleUnit().xIndex;
                if (testValue < output[0])
                    output[0] = testValue;
                if (testValue > output[1])
                    output[1] = testValue;

                // Test the minimum/maximum Y value
                testValue = tempScript.GetPuzzleUnit().yIndex;
                if (testValue < output[2])
                    output[2] = testValue;
                if (testValue > output[3])
                    output[3] = testValue;

                // Test the minimum/maximum Z value
                testValue = tempScript.GetPuzzleUnit().zIndex;
                if (testValue < output[4])
                    output[4] = testValue;
                if (testValue > output[5])
                    output[5] = testValue;
            }

        }// end of foreach

        return output;

    }

}


/* Triangle indices for each cube face
                     * front = 4,5
                     * right = 10,11
                     * top = 2,3
                     * back = 0,1
                     * left = 8,9
                     * bottom = 6,7
                     */



// way to set the rotation of an object
//Quaternion.LookRotation(-hit.normal.normalized)
