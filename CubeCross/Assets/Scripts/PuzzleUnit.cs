using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// A list of these will be stored in a PuzzleSolution that is saved as a JSON object.
[Serializable]
public class PuzzleUnit{

    // The indices in a 3D array that this cube will be stored in when a puzzle is created.
    public int xIndex;
    public int yIndex;
    public int zIndex;

    public int iD;

    // Constructor that takes in parameters storing the corresponding cube data.
    public PuzzleUnit(int inX, int inY, int inZ, int inID)
    {
        xIndex = inX;
        yIndex = inY;
        zIndex = inZ;
        iD = inID;
    }
	
}
