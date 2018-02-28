using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestScript : MonoBehaviour {

    private int[] intArray = new int[] {0, 1, 1, 0 };
    private int count = 0;
    private string row = "";
    private string[] rowArray;

	// Use this for initialization
	void Start () {
        foreach (int element in intArray)
        {
            row += element.ToString();
            if(element == 1)
            {

            }

        }
        char[] separator = new char[] { '0' };
        rowArray = row.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        foreach(string element in rowArray)
        {
            Debug.Log(element);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
