/*
 * Storage class for the attributes of a car in the current population, use to for crossover/probability/selection
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Ideas for other attributes:
 * frame shape: x, y, z manipulation
 * Just for an interesting factor, pass colors along
 */
public class specimen{

    public float torque; // how much will the motor on the hinge joint use 'force'
    public float speed; // how fast the object will want to go
    public float weight; // weight of frame
    public float distance; // total distance traveled from start object

    public int numOfWheels; // number of wheels vehicle has
    public bool[] wheelMap; // 0 = frontleft, 1 = front right, 2 = back left, 3 = backright
    public Vector3[] wheelDims; // corresponding dimentions for those wheels
    // Constructor
    public specimen()
    {
        // set the wheel map to all false
        wheelMap = new bool[4];
        wheelDims = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            wheelMap[i] = false;
        }
        numOfWheels = 0;
    }

}

