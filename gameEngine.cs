using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameEngine : MonoBehaviour {
    public int populationSize;
    public GameObject[] population;
    public float[] fitnessScores;
    public GameObject car;
    public GameObject start;
	// Use this for initialization
	void Start () {
        generatePopulation();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    // ---------------------------------------------------------------- Genetic Algorithm functions---------------------------------------------------------------------- //
    public void generatePopulation()
    {
        population = new GameObject[populationSize];
        // This loop creates the car population, runs the simulation while calculating fitness score
        for (int i = 0; i < populationSize; i++)
        {
            // generate a car
            population[i] = generateCar();
            // give it a unique name
            population[i].name += "_" + i;
            // get a list of all the parts in the car
            Transform[] t = GameObject.Find(population[i].name).GetComponentsInChildren<Transform>();
            // change frame color to random color
            t[2].GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            // change tire size
            float size = Random.Range(0.1f, 1);
            t[1].transform.localScale = new Vector3(size, 0, size);
            t[3].transform.localScale = new Vector3(size, 0, size);
            t[4].transform.localScale = new Vector3(size, 0, size);
            t[5].transform.localScale = new Vector3(size, 0, size);
            // show the car now that it is generated -- includes fitness testing
            runSimulation(i);
        }
    }

    private void runSimulation(int index)
    {
        // Car will attempt to go through the track
        // calculate the fitness score
        fitnessScores[index] = gatherFitnessScore();
    }

    public float gatherFitnessScore()
    {
        // TODO: Calculate the score based on the cars distance
        // idea: randomly generate track to continously allow for distance testing
        return 0;
    }

    public void selectCars()
    {
        // determine highest rated cars into new array
    }
    private void crossOver()
    {
        // combined the traits of the two cars
    }
    private void evaluateTarget()
    {
        // check if the target reached a certain distance
    }
    private GameObject generateCar()
    {
        return Instantiate(car, start.transform.localPosition, Quaternion.identity);
    }
}
