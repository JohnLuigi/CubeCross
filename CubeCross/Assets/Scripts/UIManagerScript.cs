﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEditor;

public class UIManagerScript : MonoBehaviour {

    public bool isMenuOn;
    public string folderPath;
    public GameObject[] puzzleButtons;
    public GameObject buttonPrefab;
    private GameObject levelButtonHolder;

    private GameObject facesHelpGroup;
    private GameObject helpText;

    private GameObject buildingManager;
    private BuilderScript buildScript;

    private GameObject inputField;
    private GameObject puzzleSavedText;
    private GameObject overwriteTextObject;
    private float savedTextTimer = 0f;
    private bool showSavedText = false;

    public int yesNoValue = 0;
    private GameObject yesButton;
    private GameObject noButton;

    private GameObject cancelButton;

    public bool enteringInput;  // variable to track if the user is typing in information.

    public bool askingReplay;

    public CubeManager cubeMgr;

    public GameObject clueEditButton;

    public GameObject exitButton;

    // Use this for initialization
    void Start () {
        // set references to the objects to be used
        levelButtonHolder = GameObject.Find("LevelButtonHolder");

        buildingManager = GameObject.Find("BuildingManager");
        buildScript = buildingManager.GetComponent<BuilderScript>();

        yesButton = GameObject.Find("YesButton");
        noButton = GameObject.Find("NoButton");

        cancelButton = GameObject.Find("CancelButton");

        cubeMgr = GameObject.Find("GameManager").GetComponent<CubeManager>();

        clueEditButton = GameObject.Find("ClueEditButton");
        clueEditButton.SetActive(false);    // Initially hide the clue edit button.
            
        // Reference to the exit button.
        exitButton = GameObject.Find("ExitButton");

        // Initially hide the yes, no, and cancel buttons.
        yesButton.SetActive(false);
        noButton.SetActive(false);
        cancelButton.SetActive(false);

        // set menu on to initially be true
        isMenuOn = true;

        // Set the path to the folder that contains the puzzles
        folderPath = Application.streamingAssetsPath;

        // set the reference to the parent object that is holding all the help images/text
        facesHelpGroup = GameObject.Find("FacesHelpGroup");
        facesHelpGroup.SetActive(false);

        //set the reference to the help text that is visible
        // initially hide it, havingthe controls/help open
        helpText = GameObject.Find("HelpText");

        inputField = GameObject.Find("InputField");
        puzzleSavedText = GameObject.Find("PuzzleSavedText");
        overwriteTextObject = GameObject.Find("OverwriteText");
        overwriteTextObject.SetActive(false);

        //Initially hide the inputField and puzzleSavedText
        inputField.SetActive(false);
        puzzleSavedText.SetActive(false);

        enteringInput = false;  // Initialize this tracker as false since the game starts
                                // without showing the inputField.

        askingReplay = false;   // AskingReplay will be set to true after a puzzle is completed.
        // TODO
        // Make this work for android/web
        // Might need to include all this in an IENumerator-returning method
        /*
        // if the streamingAssetsPath contains the start of a web address, such as when
        // the game is built for web or android, modify the path to be read via this method
        if (folderPath.Contains("://"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(folderPath);
            yield return www.SendWebRequest();
            folderPath = www.downloadHandler.text;
        }
        */

        // create a DirectoryInfo object that can be used to see files in directories
        DirectoryInfo info = new DirectoryInfo(folderPath);



        // Create an array of FileInfo objects based on the files read into the DirectoryInfo object.
        // Read them in using the searchPattern of "*.txt" to only get the files that end in ".txt"
        // so we don't try to use meta files for puzzle solutions.
        //FileInfo[] fileInfo = info.GetFiles("*.txt");
        FileInfo[] fileInfo = info.GetFiles("*.json");

        // set the size of the array that will contain the level names to be used to 
        // create buttons for each level
        puzzleButtons = new GameObject[fileInfo.Length];

        GameObject newButton;
        Vector3 startingPoint = levelButtonHolder.transform.position;
        string tempPuzzleName;

        for(int i = 0; i < fileInfo.Length; i++)
        {
            // TODO
            // create the UI elements here, or export these files or strings to the UI manager
            newButton = Instantiate(buttonPrefab, levelButtonHolder.transform) as GameObject;

            // set the starting point for th enext button to be one button's height below the next
            // can modify this to have some spacing between the buttons
            newButton.GetComponent<RectTransform>().localPosition -= 
                new Vector3(0f, (i + 1) * buttonPrefab.GetComponent<RectTransform>().rect.height, 0f);

            // remove the ".txt" from the string that will be the puzzle name
            // The name of the puzzle that will appear on the button will be 4 units shorter than its
            // current length, aka ".txt" will be removed
            //tempPuzzleName = fileInfo[i].Name.Remove(fileInfo[i].Name.Length - 4);

            // remove the ".json" from the string that will be the puzzle name
            // The name of the puzzle that will appear on the button will be 5 units shorter than its
            // current length, aka ".json" will be removed
            tempPuzzleName = fileInfo[i].Name.Remove(fileInfo[i].Name.Length - 5);
            // Set the text on the button to be displayed
            newButton.transform.GetChild(0).GetComponent<Text>().text = tempPuzzleName;
            // Give an appropriate name to each puzzle to help see them in the editor
            newButton.name = tempPuzzleName;

            puzzleButtons[i] = newButton;   // add the new button to the list of puzzle buttons

        }
    }
	
	// Update is called once per frame
	void Update () {

        if(enteringInput)
        {
            return;
        }

        // if the player presses the h key, toggle the visibility of the help screen
        if (Input.GetKeyUp("h"))
        {
            facesHelpGroup.SetActive(!facesHelpGroup.activeSelf);
            helpText.SetActive(!helpText.activeSelf);
        }

        RunSavedTextTimer();
            
    }

    // Make the buildingManager change from active to inactice, or vice versa
    public void ToggleBuildMode()
    {
        buildingManager.SetActive(!buildingManager.activeSelf);
    }

    // Method to be called in order to show/hide input field and cancel buttons, based on 
    // the input boolean parameter.
    public void DisplayTextInputField(bool input)
    {
        // Show the inputField
        inputField.SetActive(input);

        // Show the cancel button to stop trying to save the puzzle.
        cancelButton.SetActive(input);

        //TODO
        // Set the focus to be inputfield (so the user doesn't have to click on it)
    }



    // Run this in update to display the puzzleSavedText for two seconds,
    // then hide the puzzleSavedText when two seconds have run out.
    public void RunSavedTextTimer()
    {
        // If the savedText is not to be shown, do nothing.
        if (showSavedText == false)
            return;

        // If the time that has passed is less than the maximum time, increment
        // the time tracker.
        if (savedTextTimer < 2.0f)
        {
            
            // Increment the time if less than two seconds have passed
            savedTextTimer += Time.deltaTime;
        }
        // If the text is still shown after two seconds have passed
        else if(savedTextTimer >= 2.0f)
        {
            showSavedText = false;
            // Hide the puzzleSavedText object.
            puzzleSavedText.SetActive(false);
            // Reset the timer.
            savedTextTimer = 0f;
        }
    }

    // Function to be called to start the timer to show the savedTextObject.
    public void StartSavedTextTimer()
    {
        showSavedText = true;
        puzzleSavedText.SetActive(true);
    }

    // Script to take in value indicating Yes button was clicked.
    public void SetYes()
    {
        // If the yes/no buttons are being used on a puzle completed screen.
        if(askingReplay)
        {
            // Hide the puzzle completion UI.
            ShowCompletion(false);

            //TODO fix this part.
            // Somewhere in here the references to each slider gets messed up.

            // reload the puzzle
            cubeMgr.InitializeSolution(cubeMgr.puzzleBeingSolved);

            // hide the puzzle selection menu after clicking the level button
            //cubeMgr.puzzleSelector.SetActive(!cubeMgr.puzzleSelector.activeSelf);

            // Get the name of the last puzzle that was run, and re-initialize it.
            return;
        }

        // If yes was clicked, save the puzzle anyway, overwriting the old version
        // then hide the input dialog and yes/no buttons.

        yesNoValue = 1;
        // Save the puzzle here, but now with having the yesNo value set to 1, so it will overwrite
        // the old puzzle solution with the same name.
        buildScript.MakePuzzle();
        ToggleYesNo();

        SetInputStatus(false);  // Allow the rest of the UI to be used again.

    }

    // Script to take in value indicating No Button was clicked.
    public void SetNo()
    {
        // If the yes/no buttons are being used on a puzle completed screen.
        if (askingReplay)
        {
            // Show the "escape button" UI.
            cubeMgr.ToggleLevelSelectUI();

            // Hide the puzzle completion UI.
            ShowCompletion(false);

            // Return to the puzzle selection screen.
            return;
        }

        yesNoValue = 0;
        ToggleYesNo();
        // Set the saveUI reference to no longer be used so the puzzle is interactive again.
        buildScript.usingSaveUI = false;

        SetInputStatus(false);  // Allow the rest of the UI to be used again.

        // If no was clicked hide the input dialog and yes/no buttons.
        // TODO
        // Add the option to rename the file if the player chooses.
    }

    // Hides or shows the yes and no buttons, opposite of their current state.
    public void ToggleYesNo()
    {
        yesButton.SetActive(!yesButton.activeSelf);
        noButton.SetActive(!noButton.activeSelf);
        // Hide/show the overwrite Text if the yes or no button was clicked.
        ToggleOverwriteText();
    }

    // Hides the input text field and cancel buttons. Presumably run when
    // the cancel button is clicked.
    public void CancelClicked()
    {
        DisplayTextInputField(false);

        // Set the saveUI reference to no longer be used so the puzzle is interactive again.
        buildScript.usingSaveUI = false;

        SetInputStatus(false);  // Allow the rest of the UI to be used again.
        //TODO
        // clear the value that was in the inputField
    }

    // Hide or show the overwrite text.
    public void ToggleOverwriteText()
    {
        overwriteTextObject.SetActive(!overwriteTextObject.activeSelf);
    }

    // Set the enteringInput value to true or false externally.
    public void SetInputStatus(bool inputBoolean)
    {
        // If enteringInput is set to true, no UI input other than clicking buttons can be done.
        enteringInput = inputBoolean;
    }

    // Display the puzzle completed UI    
    public void ShowCompletion(bool inputBool)
    {
        SetInputStatus(inputBool);
        askingReplay = inputBool;

        // Show a replay puzzle text.
        Text tempText = overwriteTextObject.GetComponent<Text>();
        if(inputBool)
            tempText.text = "Puzzle completed!\nDo you want to replay the puzzle?";
        else
            tempText.text = "Do you want to overwrite this puzzle?";

        // Show the replay text and show the Yes/No buttons.
        ToggleYesNo();

        // If yes is clicked, start the puzzle over. If no is clicked, return to puzzle selection.
    }

    // Call this function if the exit button was clicked on.
    public void ExitButtonClicked()
    {
        //EditorApplication.ExecuteMenuItem("Edit/Play");
        Application.Quit();        
    }

}
