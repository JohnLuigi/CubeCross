using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFacesScript : MonoBehaviour {

    public Mesh mesh;
    public Vector2[] UVs;
    public float div;

    // Initializing all this stuff in Awake() because this method is called immediately when the attached object is
    // instantiated in a different script
    // The Start() method will only run after the parent's Start() method is comeplete
    private void Awake()
    {
        // set the variables that are being referenced
        mesh = GetComponent<MeshFilter>().mesh;
        UVs = new Vector2[mesh.uv.Length];
        //UVs = mesh.uv;

        if (mesh == null || UVs.Length != 24)
        {
            Debug.Log("Script needs to be attached to a Unity cube");
            return;
        }

        // division units
        // In this case we are using 128x128 cubes from a 1024x1024 texture atlas
        div = 128f / 1024f;
    }

    // Use this for initialization
    void Start () {

        // TODO to assign a specific part of a texture to a cube face, the location on the texture map needs to be saved
        // could save 4 vector2s that are the location of a specific square texture

        // order to assign textures
        // if the cube face is 
        /*
         *  2-----3
         *  |     |
         *  |     |
         *  0-----1
         */
        // Assign in the order 2, 3, 0, 1

        // for testing faces
        //SetFaces("1");

        /*
        // set the front to be red 4
        Vector2[] inputVerts = new Vector2[]{
                new Vector2(3f * div, 4f * div),
                new Vector2(4f * div, 4f * div),
                new Vector2(3f * div, 3f * div),
                new Vector2(4f * div, 3f * div) };
        */

        //mesh.uv = UVs;
    }
	
	// Update is called once per frame
	void Update () {

    }

    // Will input different strings into this function to set the according faces of the cube to certain parts
    // of its texture atlas.
    public void SetFaces(string input)
    {
        Vector2[] tempVerts;
        // TODO
        // This is only temporary, only going to set it to all 0s or all 1s for now
        if(input.Equals("0"))
        {
            // put the 4 Vector2s here, and input them into SetVertices
            // vector2s for normal 0
            tempVerts = new Vector2[]{
                new Vector2(0f * div, 8f * div),
                new Vector2(1f * div, 8f * div),
                new Vector2(0f * div, 7f * div),
                new Vector2(1f * div, 7f * div) };
            SetAllVertices(tempVerts);
        }
        else if (input.Equals("1"))
        {
            // vector2s for normal 1
            tempVerts = new Vector2[]{
                new Vector2(0f * div, 7f * div),
                new Vector2(1f * div, 7f * div),
                new Vector2(0f * div, 6f * div),
                new Vector2(1f * div, 6f * div) };
            SetAllVertices(tempVerts);
        }
        else if(input.Equals("0_D"))
        {
            // vector2s for dark 0
            tempVerts = new Vector2[]{
                new Vector2(1f * div, 8f * div),
                new Vector2(2f * div, 8f * div),
                new Vector2(1f * div, 7f * div),
                new Vector2(2f * div, 7f * div) };
            SetAllVertices(tempVerts);
        }
        else if(input.Equals("1_D"))
        {
            // vector2s for dark 1
            tempVerts = new Vector2[]{
                new Vector2(1f * div, 7f * div),
                new Vector2(2f * div, 7f * div),
                new Vector2(1f * div, 6f * div),
                new Vector2(2f * div, 6f * div) };
            SetAllVertices(tempVerts);
        }
    }

    // This will set all sides of a cube to the same texture that is derived from
    // the inpu vertices
    public void SetAllVertices(Vector2[] inputVerts)
    {
        // FRONT    2    3    0    1
        UVs[2] = inputVerts[0];
        UVs[3] = inputVerts[1];
        UVs[0] = inputVerts[2];
        UVs[1] = inputVerts[3];

        // BACK    11    10     7     6
        UVs[11] = inputVerts[0];
        UVs[10] = inputVerts[1];
        UVs[7] = inputVerts[2];
        UVs[6] = inputVerts[3];

        // RIGHT   17 18 16 19
        UVs[17] = inputVerts[0];
        UVs[18] = inputVerts[1];
        UVs[16] = inputVerts[2];
        UVs[19] = inputVerts[3];

        // LEFT 21   22    20    23
        UVs[21] = inputVerts[0];
        UVs[22] = inputVerts[1];
        UVs[20] = inputVerts[2];
        UVs[23] = inputVerts[3];

        // TOP    9    8    5    4
        UVs[9] = inputVerts[0];
        UVs[8] = inputVerts[1];
        UVs[5] = inputVerts[2];
        UVs[4] = inputVerts[3];

        // BOTTOM   15   12   14   13
        UVs[15] = inputVerts[0];
        UVs[12] = inputVerts[1];
        UVs[14] = inputVerts[2];
        UVs[13] = inputVerts[3];

        mesh.uv = UVs;
    }
}


// To flip a face vertically, swap the first two vertices with the second two vertices,
// then swap the new first with the new second, and the new third with the new fourth
// e.g.
// 2 3 0 1
//    to
// 1 0 3 2

// OLD VALUES FOR REFERENCE IF NEED BE
//   2 --- 3
//   |     |
//   |     |
//   0 --- 1

/*  theUVs[2] = Vector2( 0, 1 );
    theUVs[3] = Vector2( 1, 1 );
    theUVs[0] = Vector2( 0, 0 );
    theUVs[1] = Vector2( 1, 0 );
 * 
 * */
//    2    3    0    1   Front
//    6    7   10   11   Back
//   19   17   16   18   Left
//   23   21   20   22   Right
//    4    5    8    9   Top
//   15   13   12   14   Bottom

