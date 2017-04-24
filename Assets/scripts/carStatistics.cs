/*  This is the main script for the car object
 *  this should handle everything about the car: the velocity, torque, size, parts
 *  remove 0 wheel option...its bullshit anyway
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carStatistics : MonoBehaviour {
    // Actually stats for combustion
    public float velocity;
    public float torque;
    public Vector3 startLocation;
    public Vector3 startRotation;
    // Wheel Section 
    public int wheelCount;
    public bool[] wheelMap; // use this to show which wheel is active on this car
    public Vector3[] wheelDims; // use this in conjunction with wheel map

    //public GameObject[] wheels; // list of game objects as wheels
    public GameObject wheelObj;
    public GameObject instWheel; // instantiated tire
    // Frame Section
    public GameObject[] frames; // list of game objects as frames
    public GameObject frame; // the actual fram chosen
    public GameObject instFrame; // instantiated frame

	// Use this for initialization
	void Start () {
        // this generates a random vehicle with no genetic algorithm included - initial population
        generateRandomCar();  
    }
	private void generateRandomCar()
    {
        startLocation = this.transform.position;
        // frame generation
        int random = Random.Range(0, 100) % 2; // random selection for frame
        frame = frames[random]; // pick a random frame
        // Instantiate the frame to a current frame representation
        instFrame = Instantiate(frame, startLocation, Quaternion.identity); // representation of the current object
        
        frame.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().mass = Random.Range(1, 200); // randomized weight
        instFrame.transform.SetParent(this.transform); // set parent to this instantiation of carStatistics
        instFrame.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);// gets the frame and changes color
        
        // wheel generation
        // wheel map generation
        wheelMap = new bool[4];
        wheelDims = new Vector3[4];
        for(int i = 0; i < 4; i++)// set all to false;
        {
            wheelMap[i] = false;
        }
        wheelCount = 0;
        // access the wheel placeholder - randomly generates which wheel placeholders will be active
        int iterate = 0;
        foreach (Transform child in instFrame.transform)
            foreach (Transform grandchild in child) // check the wheel place holders
                if (Random.Range(0, 100) % 2 == 0)
                { // wheel is off
                    grandchild.gameObject.SetActive(false);
                    // leave wheel map index to false -- increase interate variable
                    iterate++;
                }
                else
                {// wheel is on
                    wheelCount++;
                    // set wheel map index to true -- increase interate variable
                    wheelMap[iterate] = true;
                    iterate++;
                }
        // now place a wheel if the placeholder exists
        iterate = 0;
        foreach (Transform child in instFrame.transform)
        {
            foreach (Transform grandchild in child)
            { // check the wheel place holders
                if (grandchild.gameObject.activeSelf)
                { // is wheel-placeholder ON/ENABLED
                    //Debug.Log("Placed wheel");
                    instWheel = Instantiate(wheelObj, grandchild.gameObject.transform.position, Quaternion.identity); // place at placeholders
                    //Rotate the wheel 90 degrees
                    instWheel.transform.Rotate(0, 180, 90);
                    // Now set the hinge joint to the FRAME
                    instWheel.GetComponent<HingeJoint>().connectedBody = instFrame.transform.GetChild(0).GetComponent<Rigidbody>();
                    // set the parent of the wheel to the Car class - they must be siblings with frame
                    instWheel.transform.parent = this.transform;
                    // random wheel size
                    instWheel.transform.localScale += new Vector3(Random.Range(0.01f, 0.5f), 0, Random.Range(0.01f, 0.5f));
                    wheelDims[iterate] = instWheel.transform.localScale; // log the dimensions into the wheelDimensions array -- no order to this array
                    iterate++;
                    // set force/speed/tailspin
                    HingeJoint hinge = instWheel.GetComponent<HingeJoint>();
                    JointMotor motor = hinge.motor;
                    motor.force = Random.Range(0, 100);
                    motor.targetVelocity = Random.Range(0, 2000);
                    motor.freeSpin = (Random.Range(0, 100) % 2 == 0) ? false : true;
                    hinge.motor = motor;
                }
            }
        }
    }
	// Update is called once per frame
	void Update () {
        //Debug.Log("Distance of " + this.gameObject.name + " : ");
	}
    
}
