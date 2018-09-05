using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour {

    public int index1;
    public int index2;
    public int index3;

    public PuzzleUnit puzzleUnit;

    public bool clueHidden;

    // Set the PuzzleUnit object attached to the cube that exsits in the scene.
    // This will be used to get the indices the cube is at in order to make new cubes and
    // determine their indices.
    public void SetPuzzleUnit(int inputX, int inputY, int inputZ, int inputID)
    {
        puzzleUnit = new PuzzleUnit(inputX, inputY, inputZ, inputID);
    }

    public PuzzleUnit GetPuzzleUnit()
    {
        return puzzleUnit;
    }

    // Use this for initialization
    void Start () {
        // Default the clueHidden status to false
        clueHidden = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
