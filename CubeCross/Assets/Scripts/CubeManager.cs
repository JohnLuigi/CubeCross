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
		
        // cube check/deletion block
        // if the player left clicks once, see if a cube is hit
        if(Input.GetMouseButtonDown(0))
        {
            checkCube();
        }

	}

    // cast a ray at the location of the mouse click
    void checkCube()
    {
        RaycastHit hitPoint;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hitPoint, Mathf.Infinity))
        {
            // if we hit a cube that is part of the solution, punish the player
            if(hitPoint.collider.tag == "KeyCube")
            {
                Debug.Log("This cube is cube needs to stay.");
            }

            // if we hit a cube that is not part of the solution, "delete" the cube
            else if(hitPoint.collider.tag == "BlankCube")
            {
                // TODO
                // make an animation for deleting a blank cube
                hitPoint.transform.gameObject.SetActive(false);
            }
        }


    }

	// initialize the cube array
	void createCubes()
	{
		// set a starting point based on the number of cubes to be created
		// this starting point should be half of the number of cubes, centered at 0,0,0
		Vector3 startingPoint = new Vector3((float) ((puzzleSize / 2.0f) * -1.0f) + 0.5f, 0, 0);

		cubeArray = new GameObject[puzzleSize];

		for (int i = 0; i < puzzleSize; i++) {
			
			GameObject newCube = Instantiate (exampleCube, startingPoint, Quaternion.identity) as GameObject;
			newCube.transform.localScale = Vector3.one;
			cubeArray [i] = newCube;

			startingPoint += new Vector3 (1.0f, 0, 0);
		}
	}
}
