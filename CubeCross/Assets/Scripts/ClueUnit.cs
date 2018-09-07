using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// A list of these pairs of integers will be stored to hide the clues
// in the dimension the list corresponds to.
[Serializable]
public class ClueUnit {

    public int index1;
    public int index2;

    // Constructor that takes in two values and sets the variables accordingly.
    public ClueUnit (int input1, int input2)
    {
        index1 = input1;
        index2 = input2;
    }
}
