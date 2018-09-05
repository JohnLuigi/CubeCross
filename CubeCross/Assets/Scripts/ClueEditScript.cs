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
                    string hitFace = GetFaceHit(hit.triangleIndex);

                    // Get the opposite face of the face that was hit.
                    string facePair = GetOppositeFaces(hitFace);

                    // If a face was properly matched to its opposite face,
                    // set
                    if(!facePair.Equals(""))
                    {
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

    // Return a string that consists of the face hit and its corresponding opposite.
    public string GetOppositeFaces(string inputString)
    {
        string facesHit = "";

        if (inputString.Equals("front") || inputString.Equals("back"))
            facesHit = "frontBack";

        if (inputString.Equals("top") || inputString.Equals("bottom"))
            facesHit = "topBottom";

        if (inputString.Equals("left") || inputString.Equals("right"))
            facesHit = "leftRight";

        return facesHit;
    }

    // This will make the cube GameObject in the parameter have its corresponding faces
    // toggled between blank and numbered.
    public void ToggleCubeFace(GameObject cubeObj, string facesToChange)
    {
        CubeScript tempScript = cubeObj.GetComponent<CubeScript>();

        PuzzleUnit puzzUnit = tempScript.GetPuzzleUnit();

        // TODO
        // Figure out if I need to save the entire 3D array of cubes
        // that have clues hidden/shown, or to have an array of cubes in
        // the outermost layers that can be used to determine which cubes
        // have their faces hidden/shown.


    }
}
