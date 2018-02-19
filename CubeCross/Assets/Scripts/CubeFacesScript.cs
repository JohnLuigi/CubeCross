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
            SetVertices(tempVerts);
        }
        else if (input.Equals("1"))
        {
            // vector2s for normal 1
            tempVerts = new Vector2[]{
                new Vector2(0f * div, 7f * div),
                new Vector2(1f * div, 7f * div),
                new Vector2(0f * div, 6f * div),
                new Vector2(1f * div, 6f * div) };
            SetVertices(tempVerts);
        }
        else if(input.Equals("0_D"))
        {
            // vector2s for dark 0
            tempVerts = new Vector2[]{
                new Vector2(1f * div, 8f * div),
                new Vector2(2f * div, 8f * div),
                new Vector2(1f * div, 7f * div),
                new Vector2(2f * div, 7f * div) };
            SetVertices(tempVerts);
        }
        else if(input.Equals("1_D"))
        {
            // vector2s for dark 1
            tempVerts = new Vector2[]{
                new Vector2(1f * div, 7f * div),
                new Vector2(2f * div, 7f * div),
                new Vector2(1f * div, 6f * div),
                new Vector2(2f * div, 6f * div) };
            SetVertices(tempVerts);
        }
    }

    public void SetVertices(Vector2[] inputVerts)
    {
        // FRONT    2    3    0    1
        UVs[2] = inputVerts[0];
        UVs[3] = inputVerts[1];
        UVs[0] = inputVerts[2];
        UVs[1] = inputVerts[3];


        // BACK    6    7   10   11
        UVs[6] = inputVerts[0];
        UVs[7] = inputVerts[1];
        UVs[10] = inputVerts[2];
        UVs[11] = inputVerts[3];

        // RIGHT   17 18 16 19
        UVs[17] = inputVerts[0];
        UVs[18] = inputVerts[1];
        UVs[16] = inputVerts[2];
        UVs[19] = inputVerts[3];

        // LEFT  23   20    22    21
        UVs[23] = inputVerts[0];
        UVs[20] = inputVerts[1];
        UVs[22] = inputVerts[2];
        UVs[21] = inputVerts[3];



        // TOP    4    5    8    9
        UVs[4] = inputVerts[0];
        UVs[5] = inputVerts[1];
        UVs[8] = inputVerts[2];
        UVs[9] = inputVerts[3];


        // BOTTOM   13   14   12   15
        UVs[13] = inputVerts[0];
        UVs[14] = inputVerts[1];
        UVs[12] = inputVerts[2];
        UVs[15] = inputVerts[3];

        mesh.uv = UVs;
    }
}


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

//reference
/*
 * 
 * void Start () {
     Mesh mesh = GetComponent<MeshFilter>().mesh;
     Vector2[] UVs = new Vector2[mesh.vertices.Length];
     // Front
     UVs[0] = new Vector2(0.0f, 0.0f);
     UVs[1] = new Vector2(0.333f, 0.0f);
     UVs[2] = new Vector2(0.0f, 0.333f);
     UVs[3] = new Vector2(0.333f, 0.333f);
     // Top
     UVs[4] = new Vector2(0.334f, 0.333f);
     UVs[5] = new Vector2(0.666f, 0.333f);
     UVs[8] = new Vector2(0.334f, 0.0f);
     UVs[9] = new Vector2(0.666f, 0.0f);
     // Back
     UVs[6] = new Vector2(1.0f, 0.0f);
     UVs[7] = new Vector2(0.667f, 0.0f);
     UVs[10] = new Vector2(1.0f, 0.333f);
     UVs[11] = new Vector2(0.667f, 0.333f);
     // Bottom
     UVs[12] = new Vector2(0.0f, 0.334f);
     UVs[13] = new Vector2(0.0f, 0.666f);
     UVs[14] = new Vector2(0.333f, 0.666f);
     UVs[15] = new Vector2(0.333f, 0.334f);
     // Left
     UVs[16] = new Vector2(0.334f, 0.334f);
     UVs[17] = new Vector2(0.334f, 0.666f);
     UVs[18] = new Vector2(0.666f, 0.666f);
     UVs[19] = new Vector2(0.666f, 0.334f);
     // Right        
     UVs[20] = new Vector2(0.667f, 0.334f);
     UVs[21] = new Vector2(0.667f, 0.666f);
     UVs[22] = new Vector2(1.0f, 0.666f);
     UVs[23] = new Vector2(1.0f, 0.334f);
     mesh.uv = UVs;
 }


    */

