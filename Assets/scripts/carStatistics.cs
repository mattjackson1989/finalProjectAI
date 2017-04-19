/*  This is the main script for the car object
 *  this should handle everything about the car: the velocity, torque, size, parts, score
 * 
 *  TODO: Seperate functionalities from start into their own functions
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carStatistics : MonoBehaviour {
    // Fitness score of car
    public float fitnessScore;
    
    // Actually stats for combustion
    public float velocity;
    public float torque;
    // Wheel Section
    public float wheelSize; // size of the instantiated wheel
    //public GameObject[] wheels; // list of game objects as wheels
    public GameObject wheelObj;
    public GameObject instWheel; // instantiated tire
    // Frame Section
    public GameObject[] frames; // list of game objects as frames
    public GameObject frame; // the actual fram chosen
    public GameObject instFrame; // instantiated frame

	// Use this for initialization
	void Start () {
        // this generates a random vehicle with no genetic algorithm included
        generateRandomCar();  
    }
	private void generateRandomCar()
    {
        
        // frame generation
        int random = Random.Range(0, 100) % 2; // random selection for frame
        frame = frames[random]; // pick a random frame
        instFrame = Instantiate(frame, this.transform.localPosition, Quaternion.identity); // representation of the current object
        
        instFrame.transform.SetParent(this.transform); // set parent to this instantiation of carStatistics
        instFrame.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);
        // wheel generation
        // access the wheel placeholder - randomly generates which wheel placeholders will be active
        foreach (Transform child in instFrame.transform)
            foreach (Transform grandchild in child) // check the wheel place holders
                if (Random.Range(0, 100) % 2 == 0) // wheel is off
                    grandchild.gameObject.SetActive(false);
        // now place a wheel if the placeholder exists
        foreach (Transform child in instFrame.transform)
        {
            foreach (Transform grandchild in child)
            { // check the wheel place holders
                if (grandchild.gameObject.activeSelf)
                { // is wheel-placeholder ON/ENABLED
                    Debug.Log("Placed wheel");
                    instWheel = Instantiate(wheelObj, grandchild.gameObject.transform.position, Quaternion.identity); // place at placeholders
                    //Rotate the wheel 90 degrees
                    instWheel.transform.Rotate(0, 0, 90);
                    // Now set the hinge joint to the FRAME
                    instWheel.GetComponent<HingeJoint>().connectedBody = instFrame.transform.GetChild(0).GetComponent<Rigidbody>();
                    // set the parent of the wheel to the Car class - they must be siblings with frame
                    instWheel.transform.parent = this.transform;
                    // random wheel size
                    instWheel.transform.localScale += new Vector3(Random.Range(0.01f, 0.5f), 0, Random.Range(0.01f, 0.5f));
                    
                }
            }
        }
        // Now place it in the population array object
       // transform.parent = this.transform;
        // after generation, isActive must be false until it is called again to run the simulation
        //this.gameObject.SetActive(false);
    }
	// Update is called once per frame
	void Update () {
		
	}
    
}
