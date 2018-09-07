using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ClueEditScript : MonoBehaviour {

    // If this is set to true, we are in edit mode and can execute the code
    //  in this script.
    public bool editingClues = false;   // initialize as false since we start NOT editing clues.
    public CubeManager cubeManager;

    // These two indices will be updated each time a cube is clicked. If the cube was set to
    // hide a clue, save these ints
    private int clueIndex1;
    private int clueIndex2;

    // This list will store the cubes that need to have their faces changed for each click.
    public List<GameObject> cubesToChange;

	// Use this for initialization
	void Start ()
    {
        // Set reference for the cubeManager script.
        cubeManager = GameObject.Find("GameManager").GetComponent<CubeManager>();

        // Initialize the cubesToChange List.
        cubesToChange = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Only execute code in ClueEdit script if it is set to edit mode.
        if (!editingClues)
            return;

        // Cast a ray every frame, in case we click on a cube face.

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // If the player does a single left click on a cube,
        // get the face of the cube that was clicked on,
        // change that face and its opposite face to be blank,
        // then repeat that action for all other cubes in the same row as that face.
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Get the tag of the object being hovered over.
                string hitTag = hit.transform.gameObject.tag;

                // If we are over a KeyCube or a BlankCube (aka not a slider)
                if (hitTag.Equals("KeyCube") || hitTag.Equals("BlankCube"))
                {

                    // Get the face that was clicked on.
                    string faceHit = GetFaceHit(hit.triangleIndex);

                    // If a face was clicked, set the column of cubes in the
                    // same face-direction as the clicked cube's.
                    if(!faceHit.Equals(""))
                    {
                        // Clear out any old cubes from a previous face being clicked.
                        cubesToChange.Clear();

                        // Save the new cubes to change.
                        cubesToChange = GetColumnOfCube(hit.transform.gameObject, faceHit);

                        // For each cube in the list of cubes to change, change its
                        // face to blank or numbered.
                        foreach(GameObject cube in cubesToChange)
                        {
                            // Toggle the face that was hit and its opposite between
                            // numbered and blank.
                            ToggleCubeFace(cube, faceHit);
                        }

                        

                        // Get the entire row of cubes (including the cube hit)
                        // and set all their faces to blank or numbered.

                        

                        // TODO
                        // Set a value for each cube to be blank or numbered.
                        // Use this value in the face-setting of puzzle creation
                        // to set cubes to have numbers or be blank.

                        //TODO
                        // When creating the cubes, set their puzzleUnit variable
                        // to be the corresponding puzzle unit from the puzzleSolution.json
                        // file. This puzzleUnit will also have a clueHidden variable
                        // that will determine if it should be shown or hidden during
                        // a normal puzzle-solving gameplay session.
                    }
                }

            }
        }

        // If the s key is pressed, save a JSON file
        if (Input.GetKeyUp(KeyCode.S))
        {
            SaveClueChanges();
        }


    }

    // TODO
    // Use the puzzle that was loaded in via the usual CubeManager.

    // Make cubeManager do nothing if the ClueEditScript is currently active.

    // Once the puzzle is loaded, check for left moues button clicks.

    // Get the face of the cube that was clicked on.

    // If the face has a number on it,
    // Set that face and its opposite face to be blank.

    // Repeat the faces setting to blank for the rest of the cubes in the
    // row behind the face that was clicked on.

    // If the face is blank, 
    // Set that face and its opposite face to the number
    // it was originally saved with.

    // Repeat the faces setting to a number for the rest of the cubes in the
    // row behind the face that was clicked on.

    // Possibly TODO later:
    // Make all the solution cubes a different color in order to help
    // visualizing the puzzle while editing clues.
    // Have a toggle button to be able to turn on/off the solution cubes colors.

    // This gives us the face and its opposite
    // that were hit based on the input triangleIndex integer.
    public string GetOppositeFacesHit(int hitTriangle)
    {
        string faceHit = "";

        if (hitTriangle == 0 || hitTriangle == 1 || hitTriangle == 4 || hitTriangle == 5)
            faceHit = "front_back";

        if (hitTriangle == 2 || hitTriangle == 3 || hitTriangle == 6 || hitTriangle == 7)
            faceHit = "top_bottom";
        
        if (hitTriangle == 8 || hitTriangle == 9 || hitTriangle == 10 || hitTriangle == 11)
            faceHit = "left_right";

        return faceHit;
    }

    // This returns a string of the face that was hit based on the input integer.
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

    // This will make the cube GameObject in the parameter have its corresponding faces
    // toggled between blank and numbered.
    public void ToggleCubeFace(GameObject cubeObj, string facesToChange)
    {
        //CubeScript tempScript = cubeObj.GetComponent<CubeScript>();

        //PuzzleUnit puzzUnit = tempScript.GetPuzzleUnit();

        CubeFacesScript tempFacesScript = cubeObj.GetComponent<CubeFacesScript>();

        //TODO
        // Get the color to set the cube to be light or dark gray to
        // maintain the checkerboard pattern.

        // This will be the status of the faces to hide before being clicked on.
        // Default to false.
        bool facesToHideStatus = false;

        // Determine which pair of faces need to be checked to see if they
        // have their clues shown or not.
        if (facesToChange.Equals("front") || facesToChange.Equals("back"))
        {
            facesToHideStatus = tempFacesScript.frontBackClueHidden;
        }
        if (facesToChange.Equals("top") || facesToChange.Equals("bottom"))
        {
            facesToHideStatus = tempFacesScript.topBottomClueHidden;
        }
        if (facesToChange.Equals("left") || facesToChange.Equals("right"))
        {
            facesToHideStatus = tempFacesScript.leftRightClueHidden;
        }

        

        // If the cube face is currently set to be hidden (within the context of the 
        // clue editor mode being active), show the original puzzle faces.
        if(facesToHideStatus)
        {            
            // If the faces are numbered, set the faces to gray
            // and set the corresponding facePairClueHidden value in the CubeFacesScript
            // to false.
            if (facesToChange.Equals("front") || facesToChange.Equals("back"))
            {
                tempFacesScript.SetFace("front", tempFacesScript.frontBack);
                tempFacesScript.SetFace("back", tempFacesScript.frontBack);

                tempFacesScript.frontBackClueHidden = false;
            }
            if (facesToChange.Equals("top") || facesToChange.Equals("bottom"))
            {
                tempFacesScript.SetFace("top", tempFacesScript.topBottom);
                tempFacesScript.SetFace("bottom", tempFacesScript.topBottom);

                tempFacesScript.topBottomClueHidden = false;
            }
            if (facesToChange.Equals("left") || facesToChange.Equals("right"))
            {
                tempFacesScript.SetFace("left", tempFacesScript.leftRight);
                tempFacesScript.SetFace("right", tempFacesScript.leftRight);

                tempFacesScript.leftRightClueHidden = false;
            }

        }
        // Otherwise if the clues on this face are set to be shown, set the faces
        // to be hidden (blank).
        else
        {
            // If the faces are numbered, set the faces to gray
            // and set the corresponding facePairClueHidden value in the CubeFacesScript
            // to true.
            if (facesToChange.Equals("front") || facesToChange.Equals("back"))
            {
                tempFacesScript.SetFace("front", "LightGray");
                tempFacesScript.SetFace("back", "LightGray");

                tempFacesScript.frontBackClueHidden = true;
            }
            if (facesToChange.Equals("top") || facesToChange.Equals("bottom"))
            {
                tempFacesScript.SetFace("top", "LightGray");
                tempFacesScript.SetFace("bottom", "LightGray");

                tempFacesScript.topBottomClueHidden = true;
            }
            if (facesToChange.Equals("left") || facesToChange.Equals("right"))
            {
                tempFacesScript.SetFace("left", "LightGray");
                tempFacesScript.SetFace("right", "LightGray");

                tempFacesScript.leftRightClueHidden = true;
            }

        }
        


        // TODO
        // Figure out if I need to save the entire 3D array of cubes
        // that have clues hidden/shown, or to have an array of cubes in
        // the outermost layers that can be used to determine which cubes
        // have their faces hidden/shown.


    }

    // This returns a list of the cubes that are "behind" the cube that was clicked
    // on. Essentially the rest of the cubes in the direction of the clicked-on face.
    public List<GameObject> GetColumnOfCube(GameObject cubeObj, string faceClicked)
    {
        // Initialize the list to return.
        List<GameObject> columnOfCubes = new List<GameObject>();

        // Find the dimension to check in based on
        // the input parameter faceClicked.

        /*
         * Front = +z axis
         * Back = -z axis
         * 
         * Top = +y axis
         * Bottom = -y axis
         * 
         * Left = +x axis
         * Right = -x axis
         * 
         * */

        // Get the z,y,x values from the cube that is clicked on via
        // the CubeScript attached to the cube.        

        // This will be the number of cubes in the respective direction based on
        // the size of the puzzle in that direction (x, y, z).
        int numberOfCubesInColumn = 0;

        // If front/back is clicked, get the input cube's y and x axis indexes
        // and set the index to iterate through to be the Z index.
        if (faceClicked.Equals("front") || faceClicked.Equals("back"))
        {
            numberOfCubesInColumn = cubeManager.puzzleSize_Z;

            int yIndex = cubeObj.GetComponent<CubeScript>().index2;
            int xIndex = cubeObj.GetComponent<CubeScript>().index3;

            for (int i = 0; i < numberOfCubesInColumn; i++)
            {
                columnOfCubes.Add(cubeManager.cubeArray[i, yIndex, xIndex]);
            }

        }

        // If top/bottom is clicked, get the input cube's z and x axis indexes
        // and set the index to iterate through to be the Y index.
        if (faceClicked.Equals("top") || faceClicked.Equals("bottom"))
        {
            numberOfCubesInColumn = cubeManager.puzzleSize_Y;

            int zIndex = cubeObj.GetComponent<CubeScript>().index1;
            int xIndex = cubeObj.GetComponent<CubeScript>().index3;

            for (int i = 0; i < numberOfCubesInColumn; i++)
            {
                columnOfCubes.Add(cubeManager.cubeArray[zIndex, i, xIndex]);
            }

        }

        // If left/right is clicked, get the input cube's z and y axis indexes
        // and set the index to iterate through to be the X index.
        if (faceClicked.Equals("left") || faceClicked.Equals("right"))
        {
            numberOfCubesInColumn = cubeManager.puzzleSize_X;

            int zIndex = cubeObj.GetComponent<CubeScript>().index1;
            int yIndex = cubeObj.GetComponent<CubeScript>().index2;

            for (int i = 0; i < numberOfCubesInColumn; i++)
            {
                columnOfCubes.Add(cubeManager.cubeArray[zIndex, yIndex, i]);
            }

        }

        return columnOfCubes;
    }

    // Method to save the changes made to the puzzle's clues. This will create a new
    // saved file with a similar name as the non-clue-edited file.
    public void SaveClueChanges()
    {
        // Search through each of the 3 main faces of the puzzle,
        // the XY-face (front-back) z is 0
        // the YZ-face (left-right) x is 0
        // the XZ-face (top-bottom) y is 0
        // and see if each cube's respective faceHidden value in CubeFacesScript
        // is set to true.

        // If it is set to true, store the 2 values used to get to that
        // cube in the respective list of the 3 lists of clueUnits
        // and write them to a puzzleSolution.json file.        

        CubeFacesScript tempFacesScript;

        //++++++++++++++++++++++++++++Z FACES BLOCK+++++++++++++++++++++++++++++++
        // Block of zFaces

        List<ClueUnit> zFaces = new List<ClueUnit>();

        // Search XY face, with z being 0 since its the "front" face of the puzzle.
        for (int i = 0; i < cubeManager.puzzleSize_Y; i++)
        {
            for(int j = 0; j < cubeManager.puzzleSize_X; j++)
            {
                // If the cube at that index has its clue hidden, store
                // the Y and X values in the zFaces list to be saved in the json file.
                tempFacesScript = cubeManager.cubeArray[0, i, j].GetComponent<CubeFacesScript>();
                if (tempFacesScript.frontBackClueHidden)
                {
                    zFaces.Add(new ClueUnit(i, j));
                }
                
            }
        }

        //++++++++++++++++++++++++++++Y FACES BLOCK+++++++++++++++++++++++++++++++
        // Block of yFaces
        List<ClueUnit> yFaces = new List<ClueUnit>();

        // Search XZ face, with y being 0 since its the "bottom" face of the puzzle.
        for (int i = 0; i < cubeManager.puzzleSize_Z; i++)
        {
            for (int j = 0; j < cubeManager.puzzleSize_X; j++)
            {
                // If the cube at that index has its clue hidden, store
                // the Z and X values in the yFaces list to be saved in the json file.
                tempFacesScript = cubeManager.cubeArray[i, 0, j].GetComponent<CubeFacesScript>();
                if (tempFacesScript.topBottomClueHidden)
                {
                    yFaces.Add(new ClueUnit(i, j));
                }

            }
        }

        //++++++++++++++++++++++++++++X FACES BLOCK+++++++++++++++++++++++++++++++
        // Block of xFaces
        List<ClueUnit> xFaces = new List<ClueUnit>();

        // Search YZ face, with x being 0 since its the "left" face of the puzzle.
        for (int i = 0; i < cubeManager.puzzleSize_Z; i++)
        {
            for (int j = 0; j < cubeManager.puzzleSize_Y; j++)
            {
                // If the cube at that index has its clue hidden, store
                // the Z and Y values in the xFaces list to be saved in the json file.
                tempFacesScript = cubeManager.cubeArray[i, j, 0].GetComponent<CubeFacesScript>();
                if (tempFacesScript.leftRightClueHidden)
                {
                    xFaces.Add(new ClueUnit(i, j));
                }

            }
        }

        // Set the zFaces list in the puzzleSolution from the CubeManager
        // to the newly created zFaces list here.
        cubeManager.loadedSolution.zFacesClues = zFaces;

        // Do the same for the yFaces list.
        cubeManager.loadedSolution.yFacesClues = yFaces;

        // Do the same for the xFaces list.
        cubeManager.loadedSolution.xFacesClues = xFaces;

        //TODO 
        // possibly reuse the same list for x,y,z faces to save memory if needed.

        // Save the newly updated puzzleSolution.
        // Create a string that is made from the PuzzleSolution object.
        string jsonString = JsonUtility.ToJson(cubeManager.loadedSolution, true);

        // New file name
        string newFileName = cubeManager.puzzleBeingSolved + "_Clued.json";
        
        // TODO
        // check for permission to overwrite somewhere around here.

        // The path to the file (will be in the StreamingAssets folder)
        string filePath = Path.Combine(Application.streamingAssetsPath, newFileName);

        // Write the json string to the file with the new updated name.
        File.WriteAllText(filePath, jsonString);

        // Refreshes the assets in the Editor so you can immediately see the new file made.
        //AssetDatabase.Refresh();

        //TODO
        // Add a display notification that the editedClues version was saved.
    }
}
