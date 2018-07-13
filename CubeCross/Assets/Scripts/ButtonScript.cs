using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

    private CubeManager cubeMgr;
    private string puzzleName;

	// Use this for initialization
	void Start () {

        // reference to the cubeManager in order to run methods from the manager
        cubeMgr = GameObject.Find("GameManager").GetComponent<CubeManager>();
                

        // get the name of the puzzle to load from the text object attached to the button
        puzzleName = transform.GetChild(0).GetComponent<Text>().text;

        // create a reference to the button component attached to the object this script is
        // attached to
        Button bttn = this.GetComponent<Button>();
        bttn.onClick.AddListener(TaskOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TaskOnClick()
    {
        // load the puzzle
        cubeMgr.InitializePuzzle(puzzleName);
        // hide the puzzle selection menu after clicking the level button
        cubeMgr.puzzleSelector.SetActive(!cubeMgr.puzzleSelector.activeSelf);

    }


}
