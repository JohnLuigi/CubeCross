using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {

    public bool isMenuOn = true;
    public string folderPath;
    public GameObject[] puzzleButtons;
    public GameObject buttonPrefab;
    private GameObject levelButtonHolder;

    private GameObject facesHelpGroup;
    private GameObject helpText;

    private GameObject buildingManager;

    private GameObject inputField;
    private GameObject puzzleSavedText;
    private float savedTextTimer = 0f;
    private bool showSavedText = false;

    // Use this for initialization
    void Start () {
        // set references to the objects to be used
        levelButtonHolder = GameObject.Find("LevelButtonHolder");

        buildingManager = GameObject.Find("BuildingManager");

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

        //Initially hide the inputField and puzzleSavedText
        inputField.SetActive(false);
        puzzleSavedText.SetActive(false);

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
        FileInfo[] fileInfo = info.GetFiles("*.txt");

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
            tempPuzzleName = fileInfo[i].Name.Remove(fileInfo[i].Name.Length - 4);
            // Set the text on the button to be displayed
            newButton.transform.GetChild(0).GetComponent<Text>().text = tempPuzzleName;
            // Give an appropriate name to each puzzle to help see them in the editor
            newButton.name = tempPuzzleName;

            puzzleButtons[i] = newButton;   // add the new button to the list of puzzle buttons

        }
    }
	
	// Update is called once per frame
	void Update () {

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

    // Method to be called in order to show input field, get its text, and then hide the
    // inputField.
    public void DisplayTextInputField()
    {
        // Show the inputField
        inputField.SetActive(true);

        //TODO
        // Set the focus to be inputfield (so the user doesn't have to click on it)
    }

    // Run this in update to display the puzzleSavedText for two seconds,
    // then hide the puzzleSavedText when two seconds have run out.
    public void RunSavedTextTimer()
    {
        if(showSavedText == true && savedTextTimer < 2.0f)
        {
            // Increment the time if less than two seconds have passed
            savedTextTimer += Time.deltaTime;
        }
        // If the text is still shown after two seconds have passed
        else if(showSavedText == true)
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
    }
}
