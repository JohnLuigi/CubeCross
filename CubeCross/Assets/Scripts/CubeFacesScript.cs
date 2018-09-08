using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFacesScript : MonoBehaviour {

    public Mesh mesh;
    public Vector2[] UVs;
    public float div;
    // TODO
    // use this
    public string[] facesArray; // this will possibly store the state of each face (texture, highlightef or not)
    public bool dark = false;   // This value will track whether a cube is to be a dark or light cube
                                // to create the checkerboard pattern
    
    // These 3 strings will store the three textures that will define the 6 faces of the cube
    // They will be used when changing the cube to flagged or un-flagged
    public string frontBack;
    public string topBottom;
    public string leftRight;

    public bool flagged = false;

    public bool frontBackClueHidden;
    public bool topBottomClueHidden;    
    public bool leftRightClueHidden;

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
        div = 120f / 1440f;

        // Default the clueHidden values to false.
        frontBackClueHidden = false;
        topBottomClueHidden = false;
        leftRightClueHidden = false;

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
    /*
    SetFace("front", "0");
    SetFace("back", "1");
    SetFace("left", "0_D");
    SetFace("right", "1_D");
    SetFace("top", "x");
    SetFace("bottom", "0");
    */
}
	
	// Update is called once per frame
	void Update () {
        
    }

    // This method will set a specific cube's face to be a specific texture on the texture map for the cubes
    public void SetFace(string face, string texture)
    {
        Vector2[] faceVerts = FaceStringToVertices(texture);

        switch (face)
        {
            case "front":
                // FRONT    2    3    0    1
                UVs[2] = faceVerts[0];
                UVs[3] = faceVerts[1];
                UVs[0] = faceVerts[2];
                UVs[1] = faceVerts[3];
                break;

            case "back":
                // BACK    11    10     7     6
                UVs[11] = faceVerts[0];
                UVs[10] = faceVerts[1];
                UVs[7] = faceVerts[2];
                UVs[6] = faceVerts[3];
                break;

            case "left":
                // LEFT 21   22    20    23
                UVs[21] = faceVerts[0];
                UVs[22] = faceVerts[1];
                UVs[20] = faceVerts[2];
                UVs[23] = faceVerts[3];
                break;

            case "right":
                // RIGHT   17 18 16 19
                UVs[17] = faceVerts[0];
                UVs[18] = faceVerts[1];
                UVs[16] = faceVerts[2];
                UVs[19] = faceVerts[3];
                break;

            case "top":
                // TOP    9    8    5    4
                UVs[9] = faceVerts[0];
                UVs[8] = faceVerts[1];
                UVs[5] = faceVerts[2];
                UVs[4] = faceVerts[3];
                break;

            case "bottom":
                // BOTTOM   15   12   14   13
                UVs[15] = faceVerts[0];
                UVs[12] = faceVerts[1];
                UVs[14] = faceVerts[2];
                UVs[13] = faceVerts[3];
                break;

            case "":
                // An empty face was input, do nothing.
                break;

            default:
                Debug.Log("You tried to use an invalid side.");
                break;
        }

        mesh.uv = UVs;
    }

    
    // Outputs the corresponding 4 vertices on the texture for the input string
    public Vector2[] FaceStringToVertices(string input)
    {
        Vector2[] output;
        // If the input string is a specific color/text, do that instead of the mathematical
        // version of setting the texture coordinates.
        if (input.Equals("LightGray"))
        {
            output = new Vector2[]
            {
                new Vector2(0f * div, 1f * div),
                new Vector2(1f * div, 1f * div),
                new Vector2(0f * div, 0f * div),
                new Vector2(1f * div, 0f * div)
            };

            return output;
        }
        else if(input.Equals("DarkGray"))
        {
            output = new Vector2[]
            {
                new Vector2(1f * div, 1f * div),
                new Vector2(2f * div, 1f * div),
                new Vector2(1f * div, 0f * div),
                new Vector2(2f * div, 0f * div)
            };

            return output;
        }
        else if(input.Equals("Red"))
        {
            output = new Vector2[]
            {
                new Vector2(7f * div, 1f * div),
                new Vector2(8f * div, 1f * div),
                new Vector2(7f * div, 0f * div),
                new Vector2(8f * div, 0f * div)
            };

            return output;
        }

        string[] textureInfo = input.Split('_');

        // Possible textures for a number (12 different ones)
        // Examples for 0

        // normal = 0
        // normal dark = 0_D
        // circled  = 0_C
        // circled dark = 0_C_D
        // squared  = 0_S
        // squared dark = 0_S_D
        // flagged  = 0_F
        // flagged dark =   0_F_D
        // flagged circled = 0_C_F
        // flagged circled dark = 0_C_F_D
        // flagged squared  =   0_S_F
        // flagged squared dark =   0_S_F_D

        // the number that will be on the face of the cube, default to 0
        int number = 0;

        // each number will start at an x-axis value of zero since that is the normal number texture
        int xStart = 0;

        // go through the texture info and see if a certain index has a certain character
        for(int i = 0; i < textureInfo.Length; i++)
        {            
            // the first element of the string will be the number on the texture
            if(i == 0)
            {
                number = int.Parse(textureInfo[i]);
            }
            else
            {
                // Dark = +1
                // Circle = +2
                // Square = +4
                // Flagged = +6
                
                // Get the string element and add to the starting x value depending on the letter
                if(textureInfo[i].Equals("D"))
                {
                    xStart += 1;
                }
                else if (textureInfo[i].Equals("C"))
                {
                    xStart += 2;
                }
                else if (textureInfo[i].Equals("S"))
                {
                    xStart += 4;
                }
                else if (textureInfo[i].Equals("F"))
                {
                    xStart += 6;
                }
            }
                
        }

        // format for each texture is
        // 1----2
        // |    |
        // 3----4

        // there are 12 rows and 12 columns in the cube texture so we are using units of 12
        // for each possible cube face. Refer to the CubeFaces_12x12.png texture in the Textures folder
        int divisions = 12;

        // the four coordinates that will make up the UV vertices for the texture
        // laid out like this
        // xLeft, yTop ----------------- xRight, yTop
        //   |                                    |
        //   |                                    |
        //   |                                    |
        //   |                                    |
        //   |                                    |
        //   |                                    |
        //   |                                    |
        //   |                                    |
        // xLeft, yBottom -------------- xRight, yBottom

        float xLeft = xStart * div;
        float xRight = (xStart + 1) * div;
        float yTop = (divisions - number) * div;
        float yBottom = (divisions - number - 1) * div;

        //TODO see if taking out that comma on the 4th vector is okay
        output = new Vector2[]
        {
            new Vector2(xLeft, yTop),
            new Vector2(xRight, yTop),
            new Vector2(xLeft, yBottom),
            new Vector2(xRight, yBottom)
        };

        return output;
    }

    // This will return an array of Vector2s that contain the necessary vertices to
    // map a specific texture
    public Vector2[] GetFaceVerts(string input)
    {
        // TODO
        // Make this default to a blank cube
        // default the face to be a blank cube face
        Vector2[] returnArray = new Vector2[]{
                new Vector2(0f * div, 1f * div),
                new Vector2(1f * div, 1f * div),
                new Vector2(0f * div, 0f * div),
                new Vector2(1f * div, 0f * div) };

        // set the returnArray to have the relevant coordinates from the texture map
        switch (input)
        {
            case "0":
                returnArray = new Vector2[]{
                new Vector2(0f * div, 12 * div),
                new Vector2(1 * div, 12 * div),
                new Vector2(0f * div, 11 * div),
                new Vector2(1f * div, 11f * div) };
                break;

            case "0_D":
                returnArray = new Vector2[]{
                new Vector2(1f * div, 12 * div),
                new Vector2(2f * div, 12 * div),
                new Vector2(1f * div, 11 * div),
                new Vector2(2f * div, 11 * div) };
                break;

            case "1":
                returnArray = new Vector2[]{
                new Vector2(0f * div, 11 * div),
                new Vector2(1f * div, 11 * div),
                new Vector2(0f * div, 10 * div),
                new Vector2(1f * div, 10 * div) };
                break;

            case "1_D":
                returnArray = new Vector2[]{
                new Vector2(1f * div, 11 * div),
                new Vector2(2f * div, 11 * div),
                new Vector2(1f * div, 10 * div),
                new Vector2(2f * div, 10 * div) };
                break;

            case "2":
                returnArray = new Vector2[]{
                new Vector2(0f * div, 6f * div),
                new Vector2(1f * div, 6f * div),
                new Vector2(0f * div, 5f * div),
                new Vector2(1f * div, 5f * div) };
                break;

            case "2_D":
                returnArray = new Vector2[]{
                new Vector2(1f * div, 6f * div),
                new Vector2(2f * div, 6f * div),
                new Vector2(1f * div, 5f * div),
                new Vector2(2f * div, 5f * div) };
                break;

            case "3":
                returnArray = new Vector2[]{
                new Vector2(0f * div, 5f * div),
                new Vector2(1f * div, 5f * div),
                new Vector2(0f * div, 4f * div),
                new Vector2(1f * div, 4f * div) };
                break;

            case "3_D":
                returnArray = new Vector2[]{
                new Vector2(1f * div, 5f * div),
                new Vector2(2f * div, 5f * div),
                new Vector2(1f * div, 4f * div),
                new Vector2(2f * div, 4f * div) };
                break;

            case "4":
                returnArray = new Vector2[]{
                new Vector2(0f * div, 4f * div),
                new Vector2(1f * div, 4f * div),
                new Vector2(0f * div, 3f * div),
                new Vector2(1f * div, 3f * div) };
                break;

            case "4_D":
                returnArray = new Vector2[]{
                new Vector2(1f * div, 4f * div),
                new Vector2(2f * div, 4f * div),
                new Vector2(1f * div, 3f * div),
                new Vector2(2f * div, 3f * div) };
                break;

            default:
                Debug.Log("The face you gave was invalid. Cube face was set to blank.");
                break;
        }

        return returnArray;
    }

    // Will input different strings into this function to set the according faces of the cube to certain parts
    // of its texture atlas.
    public void SetAllFaces(string input)
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
    // the input vertices
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

    // Public method to set the cluehidden variables to true/false.
    public void SetFacesHidden(string inputString, bool inputBool)
    {
        if(inputString.Equals("frontback"))
        {
            frontBackClueHidden = inputBool;
        }

        if (inputString.Equals("topbottom"))
        {
            topBottomClueHidden = inputBool;
        }

        if (inputString.Equals("leftright"))
        {
            leftRightClueHidden = inputBool;
        }
    }

    // Public method to change a face to red blank face for hidden and flagged
    // cube faces.
    public void ToggleFlaggedClue(string inputString, bool flagStatus)
    {
        //TODO
        // Determine if dark or light red or gray need to be used.
        //string redType = "Red";
        // TODO Add a dark red to the setFace options.

        string grayType = "LightGray";
        if (dark)
            grayType = "DarkGray";

        if (inputString.Equals("frontback"))
        {
            // If the face pair is flagged, set to blank red.
            if(flagStatus)
            {
                SetFace("front", "Red");
                SetFace("back", "Red");
            }
            // If the face pair is not flagged, set to blank gray.
            // TODO make this alternate between dark and light gray.
            // Might have to instead apply the red above.
            // maybe check the isDark variable for the cube.
            else
            {
                SetFace("front", grayType);
                SetFace("back", grayType);
            }
            
        }

        if (inputString.Equals("topbottom"))
        {
            // If the face pair is flagged, set to blank red.
            if (flagStatus)
            {
                SetFace("top", "Red");
                SetFace("bottom", "Red");
            }
            // If the face pair is not flagged, set to blank gray.
            // TODO make this alternate between dark and light gray.
            // Might have to instead apply the red above.
            // maybe check the isDark variable for the cube.
            else
            {
                SetFace("top", grayType);
                SetFace("bottom", grayType);
            }

        }

        if (inputString.Equals("leftright"))
        {
            // If the face pair is flagged, set to blank red.
            if (flagStatus)
            {
                SetFace("left", "Red");
                SetFace("right", "Red");
            }
            // If the face pair is not flagged, set to blank gray.
            // TODO make this alternate between dark and light gray.
            // Might have to instead apply the red above.
            // maybe check the isDark variable for the cube.
            else
            {
                SetFace("left", grayType);
                SetFace("right", grayType);
            }

        }

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

