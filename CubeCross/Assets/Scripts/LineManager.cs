using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour {

    GameObject gameManager; // Reference to hte game manager script where we will get our bounds info
    CubeManager cubeManager;    // Reference to the script attached to the gameManager
    LineRenderer lineRenderer;  // Reference to the LineRenderer component added to this object

    Vector3 puzzleCenter;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager");
        cubeManager = gameManager.GetComponent<CubeManager>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 19;     // There are 8 vertices we want the line to run through
                                             // we'll along the bottom face, up one corner, along the top face
        puzzleCenter = gameManager.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        // Have the same rotation as the gameManager object (so when we rotate the puzzle, the bounds
        // box also gets rotated).
        transform.rotation = gameManager.transform.rotation;
        puzzleCenter = gameManager.transform.position;
    }

    // update the new points where the lines have to be drawn
    public void UpdateLineBounds()
    {
        puzzleCenter = gameManager.transform.position;

        float offset = 1.06f;

        float xSize = cubeManager.xBoundSize + offset;
        float ySize = cubeManager.yBoundSize + offset;
        float zSize = cubeManager.zBoundSize + offset;

        Vector3[] points = 
            {
                // bottom face
                // front bottom left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z + zSize / 2f),
                // front bottom right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z + zSize / 2f),
                // back bottom right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z - zSize / 2f),
                // back bottom left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z - zSize / 2f),
                // return to front bottom left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z + zSize / 2f),

                // first leg
                // front top left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z + zSize / 2f),
                // return to front bottom left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z + zSize / 2f),

                // second leg
                // front bottom right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z + zSize / 2f),
                // front top right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z + zSize / 2f),
                // return to front bottom right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z + zSize / 2f),

                // third leg
                // back bottom right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z - zSize / 2f),
                // back top right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z - zSize / 2f),
                // return to back bottom right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z - zSize / 2f),

                //fourth leg
                // back bottom left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y - ySize / 2f, puzzleCenter.z - zSize / 2f),
                // back top left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z - zSize / 2f),

                // top face
                // front top left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z + zSize / 2f),
                // front top right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z + zSize / 2f),
                // back top right
                new Vector3 (puzzleCenter.x + xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z - zSize / 2f),
                // back top left
                new Vector3 (puzzleCenter.x - xSize / 2f, puzzleCenter.y + ySize / 2f, puzzleCenter.z - zSize / 2f),

            };

        lineRenderer.SetPositions(points);
    }
}
