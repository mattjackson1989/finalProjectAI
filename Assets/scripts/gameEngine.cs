using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameEngine : MonoBehaviour {
   
   

    public static int populationSize;
    public GameObject[] population;
    
    public static float[] fitnessScores;
    // if the car has hit the ground
    public static bool hasStarted;
    // if the generation is done
    public bool genOver;
    public GameObject car; // car game object
    public GameObject start;
    
    public int currentCar; // index of the population array that is currently running simulation
    static public bool simRunning;
	// Use this for initialization
	void Start () {
        
        populationSize = 3;
        currentCar = 0;
        generatePopulation(); // generate the population
        hasStarted = false;
        runSimulation();
    }
	
	// Update is called once per frame
	void Update () {
        
        if(hasStarted == true && genOver != true)
        {
            if(population[currentCar].transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.1)
            {
                Debug.Log("Stopped sim");
                // TODO: before setting inactive, you must record the fitness score of the car
                population[currentCar].gameObject.SetActive(false);
                currentCar++; // increase to next car in the array
                if (currentCar != populationSize)
                {
                    generatePopulation();
                    hasStarted = false;
                    runSimulation();
                }else
                {
                    hasStarted = false;
                    genOver = true;
                    Debug.Log("Generation Over");
                }
            }          
        }else
        {
            if(genOver != true)
                runSimulation(); // let a single car run a simulation
        }
        
    }
    // ---------------------------------------------------------------- Genetic Algorithm functions---------------------------------------------------------------------- //
    public void generatePopulation()
    {
        population = new GameObject[populationSize];
        fitnessScores = new float[populationSize];
        // This loop creates the car population, runs the simulation while calculating fitness score
        for (int i = 0; i < populationSize; i++)
        {
            // generate a car
            population[i] = generateCar();
            // give it a unique name
            population[i].name += "_" + i;
            population[i].gameObject.SetActive(false);
            
        }
        
    }

    private void runSimulation()
    {
        // Car will attempt to go through the track
        population[currentCar].gameObject.SetActive(true);
        // calculate the fitness score
        
        //fitnessScores[index] = gatherFitnessScore();
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
