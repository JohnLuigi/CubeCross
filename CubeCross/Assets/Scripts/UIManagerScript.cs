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

    // Use this for initialization
    void Start () {
        levelButtonHolder = GameObject.Find("LevelButtonHolder");
        // Set the path to the folder that contains the puzzles
        folderPath = Application.streamingAssetsPath;

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

        for(int i = 0; i < fileInfo.Length; i++)
        {
            // TODO
            // create the UI elements here, or export these files or strings to the UI manager
            newButton = Instantiate(buttonPrefab, levelButtonHolder.transform) as GameObject;

            // set the starting point for th enext button to be one button's height below the next
            // can modify this to have some spacing between the buttons
            newButton.GetComponent<RectTransform>().localPosition -= 
                new Vector3(0f, (i + 1) * buttonPrefab.GetComponent<RectTransform>().rect.height, 0f);


            //TODO
            // remove the ".txt" from the string that will be the puzzle name

            newButton.transform.GetChild(0).GetComponent<Text>().text = fileInfo[i].Name;

            puzzleButtons[i] = newButton;

        }
    }
	
	// Update is called once per frame
	void Update () {

        

    }
}
