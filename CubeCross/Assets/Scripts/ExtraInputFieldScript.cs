using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraInputFieldScript : MonoBehaviour {

    // Reference to the inputField object
    public InputField inputFieldScript;

    public BuilderScript builderScript;

	// Use this for initialization
	public void Start () {

        // Set reference to the builderScript
        builderScript = GameObject.Find("BuildingManager").GetComponent<BuilderScript>();
        inputFieldScript = gameObject.GetComponent<InputField>();

        // Add a listener function to this object.
        // The onEndEdit listener has to have a string as its input parameter
        inputFieldScript.onEndEdit.AddListener(EnhancedOnEndEdit);
	}

    // Do the contained stuff when a string has stopped being edited in the inputField.
    public void EnhancedOnEndEdit(string text)
    {
        // Set the puzzleName string variable to be used in saving the puzzle solution
        builderScript.SetPuzzleName(text);
        builderScript.MakePuzzle();
        gameObject.SetActive(false);
    }
    // TODO, add a cancel text input and don't try to save the puzzle option.
}

// If issues with callbacks arise, check out this StackOverflow page:
// https://stackoverflow.com/questions/50211059/set-on-end-edit-event-from-a-input-box-text-mesh-pro-while-runtime
//
// Might need to remove this listener if the inputField will be used in other places
// Remove it via:
// inputFieldScript.onEndEdit.RemoveListener(EnhancedOnEndEdit);
// or
// inputFieldScript.onEndEdit.RemoveAllListeners();
