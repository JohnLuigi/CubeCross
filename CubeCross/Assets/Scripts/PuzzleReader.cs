using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PuzzleReader : MonoBehaviour
{

    public string folderPath;

    // Use this for initialization
    void Start()
    {

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

        foreach (FileInfo file in fileInfo)
        {            
            Debug.Log("There are " + fileInfo.Length + " puzzles available.");
            Debug.Log(file.Name + "\n");

            // TODO
            // create the UI elements here, or export these files or strings to the UI manager
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

}
