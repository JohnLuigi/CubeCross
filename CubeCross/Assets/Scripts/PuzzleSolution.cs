using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class stores the data needed to create a puzzle solution out of cubes.
[Serializable]
public class PuzzleSolution {

    // These are the x,y, and z dimensions of the, to be used to create the dimensions
    // of the multi-dimensional array that will store the cubes in the GameManager.
    public int xDimension;
    public int yDimension;
    public int zDimension;

    // Will use this to let the player change the puzzle or not in-game (mostly
    // for custom puzzles).
    public bool canEdit;

    // This list will contain all the indices for the cubes made in build mode.
    public List<PuzzleUnit> puzzleUnits;

}
