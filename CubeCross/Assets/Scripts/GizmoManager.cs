using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will be our own definition of Gizmos to make it simpler to draw them in a build
// while still using the normal terminology for gizmos.
public class Gizmos
{
    // This is our custom version of Gizmo.drawline
    public static void DrawLine(Vector3 a, Vector3 b, Color color)
    {
        // If the GizmoManager is not set to show, don't try to draw any lines
        if (!GizmoManager.Show)
            return;

        GizmoManager.lines.Add(new GizmoManager.GizmoLine(a, b, color));

        try
        {
            UnityEngine.Gizmos.color = color;
            UnityEngine.Gizmos.DrawLine(a, b);
        }
        catch (UnityException unityException)
        {
            // This will ignore the "UnityException: Gizmo drawing functions can only be used in OnDrawGizmos
            // and OnDrawGizmosSelected." error
            Debug.Log("Tried to draw something with the custom DrawLine method.\n" + unityException);
        }
    }

    public static void DrawBox(Vector3 position, Vector3 size, Color color)
    {
        if (!GizmoManager.Show)
            return;

        // These are the 8 points needed to define a cube to be drawn
        // Back half
        // back bottom left (assuming we are looking at 0,0,0 with the camera at a 45 degree angle between x and z axis
        Vector3 point1 = new Vector3(position.x - size.x / 2f, position.y - size.y / 2f, position.z - size.z / 2f);
        // back bottom right
        Vector3 point2 = new Vector3(position.x + size.x / 2f, position.y - size.y / 2f, position.z - size.z / 2f);
        // back top right
        Vector3 point3 = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f, position.z - size.z / 2f);
        // back top left
        Vector3 point4 = new Vector3(position.x - size.x / 2f, position.y + size.y / 2f, position.z - size.z / 2f);

        // Front half
        // front bottom left
        Vector3 point5 = new Vector3(position.x - size.x / 2f, position.y - size.y / 2f, position.z + size.z / 2f);
        // front bottom right
        Vector3 point6 = new Vector3(position.x + size.x / 2f, position.y - size.y / 2f, position.z + size.z / 2f);
        // front top right
        Vector3 point7 = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f, position.z + size.z / 2f);
        // front top left
        Vector3 point8 = new Vector3(position.x - size.x / 2f, position.y + size.y / 2f, position.z + size.z / 2f);

        // Draw all the lines using the DrawLine method defined below in GizmoManager
        // Back side of the cube
        DrawLine(point1, point2, color);
        DrawLine(point2, point3, color);
        DrawLine(point3, point4, color);
        DrawLine(point4, point1, color);

        // Front side
        DrawLine(point5, point6, color);
        DrawLine(point6, point7, color);
        DrawLine(point7, point8, color);
        DrawLine(point8, point5, color);

        // Four "connecting" lines between front and back
        DrawLine(point1, point5, color);
        DrawLine(point2, point6, color);
        DrawLine(point3, point7, color);
        DrawLine(point4, point8, color);
    }
}


// Attach this script to a camera for it to work as intended.
public class GizmoManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // This will be the line object that will define the shapes to be drawn.
    // Made up of two vectors and a color.
    public struct GizmoLine
    {
        public Vector3 a;
        public Vector3 b;
        public Color color;

        public GizmoLine(Vector3 a, Vector3 b, Color color)
        {
            this.a = a;
            this.b = b;
            this.color = color;
        }
    }

    public bool showGizmos = true;  // Will be used to show gizmos in the build
    public Material material;       // The material the GizmoLines will be made of
    // The list that will contain the GizmoLines we defined above to be drawn in the build
    internal static List<GizmoLine> lines = new List<GizmoLine>();
    
    // The method that will set gizmos to be shown
    public static bool Show
    {
        get;
        private set;
    }
	
	// Update is called once per frame
	void Update () {

        Show = showGizmos;
	}

    // This method doesn't run every frame in the editor, only in the build.
    // In order to draw "regular" gizmos in edit mode, call a gizmo method via the 
    // usual namespace e.g. UnityEngine.Gizmos.DrawLine() etc.
    private void OnPostRender()
    {
        material.SetPass(0);
        // Set GL lines to begin being drawn.
        GL.Begin(GL.LINES);

        // For each line stored in the GizmoLines list, draw each according to its color and dimensions
        for(int i = 0; i < lines.Count; i++)
        {
            GL.Color(lines[i].color);
            GL.Vertex(lines[i].a);
            GL.Vertex(lines[i].b);
        }

        // Stop drawing lines.
        GL.End();
        lines.Clear();  // clear the list of the lines to be drawn
    }
}
