using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple script for items to bob and/or rotate
public class Bobbing : MonoBehaviour
{    
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    public bool rotate = false;
    public bool bob = false;
 
    // Position Storage Variables
    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();
 
    void Start () 
    {
        // Store the starting position & rotation of the object
        posOffset = transform.position; 
    }
     
    // Update is called once per frame
    void Update () {
        // Spin object around Y-Axis
        if (rotate)
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
        }
 
        // Float up/down with a Sin()
        if (bob)
        {
            tempPos = posOffset;
            tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
        }

        transform.position = tempPos;
    }
}

