using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class stores the data needed to create a puzzle solution out of cubes.
[Serializable]
public class PuzzleSolution {

    // These are the x,y, and z dimensions of the, to be used to create the dimensions
    // of the multi-dimensional array that will store the cubes in the GameManager.
    public int xDimensionMax;
    public int xDimensionMin;

    public int yDimensionMax;
    public int yDimensionMin;

    public int zDimensionMax;
    public int zDimensionMin;

    // Will use this to let the player change the puzzle or not in-game (mostly
    // for custom puzzles).
    public bool canEdit;

    // This list will contain all the indices for the cubes made in build mode.
    public List<PuzzleUnit> puzzleUnits;

    // This list will store all the pairs of integers needed to hide front-back
    // clues in the puzzleSolution.
    public List<ClueUnit> zFacesClues;

    // This list will store all the pairs of integers needed to hide top-bottom
    // clues in the puzzleSolution.
    public List<ClueUnit> yFacesClues;

    // This list will store all the pairs of integers needed to hide left-right
    // clues in the puzzleSolution.
    public List<ClueUnit> xFacesClues;

    // Function that will create a puzzleUnit and add it to the list stored in puzzleSolution.
    public void AddCube(int inputX, int inputY, int inputZ, int inputID)
    {
        // Create the necessary puzzleUnit.
        PuzzleUnit newPuzzleUnit = new PuzzleUnit(inputX, inputY, inputZ, inputID);

        UpdateMinsMaxes(inputX, inputY, inputZ);

        // Add the puzzleUnit to the list.
        puzzleUnits.Add(newPuzzleUnit);

        //TODO
        // Add a check somewhere to make sure the cube trying to be added is within
        // the maximum possible bounds of the puzzle (<=10 in any direction).
    }

    // Add a cube by adding the whole unit, as opposed to the actual values themselves.
    public void AddUnit(PuzzleUnit inputUnit)
    {
        puzzleUnits.Add(inputUnit);
    }

    // Function that will delete the puzzleUnit with the specified ID.
    public void RemoveCube(int inputID)
    {
        foreach (PuzzleUnit unit in puzzleUnits)
        {
            if(unit.iD == inputID)
            {
                // Get the index of the desired cube.
                int unitIndex = puzzleUnits.IndexOf(unit);
                // Remove that cube from the list.
                puzzleUnits.RemoveAt(unitIndex);
                //TODO
                // Check if the puzzle removed altered the bounds of the puzzle's dimensions.
            }
        }
        
    }

    // This function takes in the input parameters and checks to see if the are
    // greater than the maximum value or less than the minimum value for each dimension.
    public void UpdateMinsMaxes(int inputX, int inputY, int inputZ)
    {
        // X comparison block.
        if (inputX > xDimensionMax)
            xDimensionMax = inputX;
        if (inputX < xDimensionMin)
            xDimensionMin = inputX;

        // Y comparison block.
        if (inputY > yDimensionMax)
            yDimensionMax = inputY;
        if (inputY < yDimensionMin)
            yDimensionMin = inputY;

        // Z Comparison block.
        if (inputZ > zDimensionMax)
            zDimensionMax = inputZ;
        if (inputZ < zDimensionMin)
            zDimensionMin = inputZ;
    }

    // Default contructor.
    // Might make another for puzzles that can be edited, with canEdit set to true.
    public PuzzleSolution()
    {
        xDimensionMax = 1;
        xDimensionMin = 1;

        yDimensionMax = 1;
        yDimensionMin = 1;

        zDimensionMax = 1;
        zDimensionMin = 1;

        canEdit = false;

        puzzleUnits = new List<PuzzleUnit>();
    }

    // Constructor that creates a PuzzleSolution with an empty puzzleUnits list.
    public PuzzleSolution(int[] inputMinMaxes, bool inputCanEdit)
    {
        xDimensionMin = inputMinMaxes[0];
        xDimensionMax = inputMinMaxes[1];

        yDimensionMin = inputMinMaxes[2];
        yDimensionMax = inputMinMaxes[3];

        zDimensionMin = inputMinMaxes[4];
        zDimensionMax = inputMinMaxes[5];
        

        canEdit = inputCanEdit;

        puzzleUnits = new List<PuzzleUnit>();
    }

}
