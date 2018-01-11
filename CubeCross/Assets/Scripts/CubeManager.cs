using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour {

	public int puzzleSize = 10;
	public GameObject[] cubeArray;
	public GameObject exampleCube;

	// create the array of cubes that will make up the puzzle
	void Start () {
		
		createCubes ();
	}

	void Update () {
		
	}

	// initialize the cube array
	void createCubes()
	{
		// set a starting point based on the number of cubes to be created
		// this starting point should be half of the number of cubes
		Vector3 startingPoint = new Vector3((float) (puzzleSize / 2.0f) * -1.0f, 0, 0);

		cubeArray = new GameObject[puzzleSize];

		for (int i = 0; i < puzzleSize; i++) {
			
			GameObject newCube = Instantiate (exampleCube, startingPoint, Quaternion.identity) as GameObject;
			newCube.transform.localScale = Vector3.one;
			cubeArray [i] = newCube;

			startingPoint += new Vector3 (1.0f, 0, 0);
		}
	}
}
