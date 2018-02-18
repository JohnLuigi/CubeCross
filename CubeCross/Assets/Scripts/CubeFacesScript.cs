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
        UVs = new Vector2[mesh.vertices.Length];

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

        /*
        // set the variables that are being referenced
        mesh = GetComponent<MeshFilter>().mesh;
        UVs = new Vector2[mesh.vertices.Length];

        if(mesh == null || UVs.Length != 24)
        {
            Debug.Log("Script needs to be attached to a Unity cube");
            return;
        }

        // division units
        // In this case we are using 128x128 cubes from a 1024x1024 texture atlas
        div = 128f / 1024f;

        */



        // TODO to assign a specific part of a texture to a cube face, the location on the texture map needs to be saved
        // could save 4 vector2s that are the location of a specific square texture

        // order to assign textures
        // if the cube face is 
        /*
         *  1-----2
         *  |     |
         *  |     |
         *  3-----4
         */
        // Assign in the order 1, 2, 3, 4

        /*
        // front
        // location of normal 0
        UVs[0] = new Vector2(0f, 1f);
        UVs[1] = new Vector2(div, 1f);
        UVs[2] = new Vector2(0f, 1f - div);
        UVs[3] = new Vector2(div, 1f - div);

        //top is UVs 4-9
        // location of dark red 4
        UVs[4] = new Vector2(0.5f - div, 0.5f);
        UVs[5] = new Vector2(0.5f, 0.5f);
        UVs[8] = new Vector2(0.5f - div, 0.5f - div);
        UVs[9] = new Vector2(0.5f, 0.5f - div);

        // back is UVs 6 - 11
        // location of light red 1
        UVs[11] = new Vector2(2 * div, 1f - div);
        UVs[10] = new Vector2(0.5f - div, 1f - div);
        UVs[7] = new Vector2(2 * div, 1f - 2*div);
        UVs[6] = new Vector2(0.5f - div, 1f - 2 * div);

        // bottom is UVs 12-15
        // location of dark red 2
        UVs[12] = new Vector2(3f * div, 6f * div);
        UVs[13] = new Vector2(4f * div, 6f * div);
        UVs[14] = new Vector2(4f * div, 5f * div);
        UVs[15] = new Vector2(3f * div, 5f * div);

        // right is UVs 16-19
        // location of normal dark 3
        UVs[16] = new Vector2(1f * div, 5f * div);
        UVs[17] = new Vector2(2f * div, 5f * div);
        UVs[18] = new Vector2(2f * div, 4f * div);
        UVs[19] = new Vector2(1f * div, 4f * div);

        // left is UVs 20-23
        // location of normal 4
        UVs[20] = new Vector2(0f * div, 4f * div);
        UVs[21] = new Vector2(1f * div, 4f * div);
        UVs[22] = new Vector2(1f * div, 3f * div);
        UVs[23] = new Vector2(0f * div, 3f * div);
        
        mesh.uv = UVs;
        */

        // for testing faces
        SetFaces("0");
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
                new Vector2(1f * div, 7f * div),
                new Vector2(0f * div, 7f * div) };
            SetVertices(tempVerts);
        }
        else if (input.Equals("1"))
        {
            // vector2s for normal 1
            tempVerts = new Vector2[]{
                new Vector2(0f * div, 7f * div),
                new Vector2(1f * div, 7f * div),
                new Vector2(7f * div, 6f * div),
                new Vector2(0f * div, 6f * div) };
            SetVertices(tempVerts);
        }
        else if(input.Equals("0_D"))
        {
            // vector2s for dark 0
            tempVerts = new Vector2[]{
                new Vector2(1f * div, 8f * div),
                new Vector2(2f * div, 8f * div),
                new Vector2(2f * div, 7f * div),
                new Vector2(1f * div, 7f * div) };
            SetVertices(tempVerts);
        }
        else if(input.Equals("1_D"))
        {
            // vector2s for dark 1
            tempVerts = new Vector2[]{
                new Vector2(1f * div, 7f * div),
                new Vector2(2f * div, 7f * div),
                new Vector2(2f * div, 6f * div),
                new Vector2(1f * div, 6f * div) };
            SetVertices(tempVerts);
        }
    }

    public void SetVertices(Vector2[] inputVerts)
    {
        for(int i = 0; i < UVs.Length; i+= 4)
        {
            UVs[i] = inputVerts[0];
            UVs[i+1] = inputVerts[1];
            UVs[i+2] = inputVerts[2];
            UVs[i+3] = inputVerts[3];
        }
        mesh.uv = UVs;
        // swap the values of 6 and 7 with 8 and 9 if need be
    }
}

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

    