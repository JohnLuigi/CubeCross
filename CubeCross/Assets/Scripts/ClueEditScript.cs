using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueEditScript : MonoBehaviour {

    // If this is set to true, we are in edit mode and can execute the code
    //  in this script.
    public bool editingClues = false;   // initialize as false since we start NOT editing clues.

	// Use this for initialization
	void Start ()
    {

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

                    Debug.Log("Hit face pair: " + faceHit);
                    // If a face was properly matched to its opposite face,
                    // set
                    if(!faceHit.Equals(""))
                    {
                        // Toggle the face that was hit and its opposite between
                        // numbered and blank.
                        ToggleCubeFace(hit.transform.gameObject, faceHit);

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
        // Otherwise ifthe clues on this face are set to be shown, set the faces
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
    public List<GameObject> GetRowBehindCube(GameObject cubeObj, string faceClicked)
    {
        // Initialize the list to return.
        List<GameObject> rowOfCubes = new List<GameObject>();

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

        //TODO
        // Continue here


        return rowOfCubes;
    }
}
