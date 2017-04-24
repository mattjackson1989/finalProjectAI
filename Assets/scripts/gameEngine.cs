/*
 * Currently: Need to work on generateNextGen() function.
 *  - selection, crossover, mutation, replace
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameEngine : MonoBehaviour {
    public float mutationRate;
    // For UI
    public Text distanceScoreGUI;
    public Text carNumberGUI;
    // Distance traveled from start object
    public float distance;
    public float frameWeight;
    // population array
    public static int populationSize;
    public bool generateNextGen;
    public bool runNextGenertion;
    public GameObject[] population;
    // mating pool
    public List<specimen> matingPool;
    public static specimen[] currentCarStatistics; // this stores Car objects that only contain stats about a car
    // if the car has hit the ground
    public static bool hasStarted; // triggered by wheel collider
    // if the generation is done
    public bool genOver; // when the generation has ended
    public GameObject car; // car game object
    public GameObject start; // use for start location
    // Flag if the program is collecting information - sorting, crossover, mating pool, etc
    public bool isCollecting;
    public int currentCar; // index of the population array that is currently running simulation
    static public bool simRunning;
    public bool FirstGeneration;

    // Use this for initialization
    void Start () {
        mutationRate = 5;
        populationSize = 10; // HARD CODE 10 for right now - don't change unless you change prop. function
        population = new GameObject[populationSize];
        currentCarStatistics = new specimen[populationSize]; // list of the current generations specimen stats
        matingPool = new List<specimen>();
        currentCar = 0;
        generatePopulation(); // generate the population
        hasStarted = false; 
        genOver = false; 
        generateNextGen = false; // flag to allow for next generation to build
        runNextGenertion = false; // flag to allow for the next generation to run simulation
        FirstGeneration = true; // flag for first generation
        runSimulation();
    }
    // Update is called once per frame
   
	void Update () {
        // check for next generation
      
        
        if (hasStarted == true && genOver != true || (population[currentCar].GetComponent<carStatistics>().wheelCount == 0))
        {
            //Debug.Log("HIT HERE");
            if (population[currentCar].transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f) // check if the car has stopped
            {
                // check distance
                Debug.Log(population[currentCar].transform.GetChild(0).transform.GetChild(0).transform.position.z - start.transform.position.z);
                Debug.Log("Stopped simulation");
                // TODO: before setting inactive, you must record the car statistics
                Debug.Log("inputting stats");
                inputIntoStatsArray();
                population[currentCar].gameObject.SetActive(false); // turn off car
                currentCar++; // increase to next car in the array
                if (currentCar != populationSize)
                {
                    if (FirstGeneration) // first generation must create its cars here
                        generatePopulation();
                    hasStarted = false;
                    runSimulation();
                }
                else
                {
                    hasStarted = false;
                    currentCar = 0;
                    genOver = true;
                    // evaluate the cars into mating pool
                    FirstGeneration = false;
                    Debug.Log("Generation Over");
                    isCollecting = true;
                }
            }
            else
            { // spit out the distance here
                if (population[currentCar].transform.childCount > 0)
                {
                    distance = population[currentCar].transform.GetChild(0).transform.GetChild(0).transform.position.z - start.transform.localPosition.z;
                    carNumberGUI.text = (currentCar + 1).ToString(); // set UI car number
                    distanceScoreGUI.text = distance.ToString(); // set distance, but add 1 to make it more intuitive for a user
                }
            }
        }
        else
        {
            if (genOver != true)
            {
                
                runSimulation(); // let a single car run a simulation
            }
        }
        if (isCollecting)
        {
            sortStatisticalCars();
            insertIntoMatingPool();
            isCollecting = false;
            generateNextGen = true;
        }// now use for all other generations after generation1
        if (generateNextGen == true)
        {
            Debug.Log("Now creating next generation");
            generateNextGeneration();
            generateNextGen = false;
            genOver = false;
            hasStarted = false;
            Debug.Log("Now Running next gneration");
            currentCar = 0;

        }

    }
    void sortStatisticalCars()
    {
        for (int i = 0; i < currentCarStatistics.Length - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (currentCarStatistics[j - 1].distance < currentCarStatistics[j].distance)
                {
                    specimen temp = currentCarStatistics[j - 1];
                    currentCarStatistics[j - 1] = currentCarStatistics[j];
                    currentCarStatistics[j] = temp;
                }
            }
        }
        for(int i = 0; i < currentCarStatistics.Length; i++)
        {
            Debug.Log("Place " + i + " " + currentCarStatistics[i].distance);
        }
    }
    // inserts the highest to lowest array index into the mating pool list. Higher rank in array, more times it is placed in the mating pool
    void insertIntoMatingPool()
    {
        int arrayIndex = 0;
      
        for (int j = 0; j < populationSize; j++)
        {
            if (arrayIndex < 5)
            {
                for (int i = 5 - arrayIndex; i >= 0; i--)
                {
                    //Debug.Log(arrayIndex);
                    matingPool.Add(currentCarStatistics[arrayIndex]);
                }
                arrayIndex++;
            }else
            {
                matingPool.Add(currentCarStatistics[arrayIndex]);
                arrayIndex++;
            }
        }
        
        // now shuffle the mating pool
        Shuffle();
       
    }
    private System.Random rng = new System.Random();
    // Fisher-Yates shuffle algorithm
    public void Shuffle()
    {
        int n = matingPool.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            specimen value = matingPool[k];
            matingPool[k] = matingPool[n];
            matingPool[n] = value;
        }
    }
    void inputIntoStatsArray() // parse the carStatistics object to the specimen object
    {   // Get distance
        currentCarStatistics[currentCar].distance = distance;
        // Get Frame weight
        currentCarStatistics[currentCar].weight = population[currentCar].transform.GetChild(0).transform.GetChild(0).GetComponent<Rigidbody>().mass;

        // Gather wheel size/shape torque and speed information here
        
        currentCarStatistics[currentCar].numOfWheels = population[currentCar].GetComponent<carStatistics>().wheelCount; // wheel count;
        // exchange maps
        if (currentCarStatistics[currentCar].numOfWheels > 0) // if the car has wheels
        {
            Debug.Log(currentCarStatistics[currentCar].numOfWheels);
            // gather wheel dimensions
            for (int i = 0; i < 4; i++)
            {
                currentCarStatistics[currentCar].wheelMap[i] = population[currentCar].GetComponent<carStatistics>().wheelMap[i];
                currentCarStatistics[currentCar].wheelDims[i] = population[currentCar].GetComponent<carStatistics>().wheelDims[i];
               // Debug.Log(currentCarStatistics[currentCar].wheelDims[i]);
            }
            // find the speed
            foreach (Transform child in population[currentCar].transform)
            {
                // check the wheel place holders, exclude frames
                if (child.gameObject.activeSelf && child.tag != "frame")
                { // is wheel-placeholder ON/ENABLED
                    currentCarStatistics[currentCar].speed = child.GetComponent<HingeJoint>().motor.targetVelocity; // set speed
                    currentCarStatistics[currentCar].torque = child.GetComponent<HingeJoint>().motor.force; // set torque
                }
            }
        }else
        {
            currentCarStatistics[currentCar].speed = 0;
            currentCarStatistics[currentCar].torque = 0;
        }
    }
    // ---------------------------------------------------------------- Genetic Algorithm functions---------------------------------------------------------------------- //
    public void generatePopulation()
    {
        // generate a car
        population[currentCar] = generateCar();
        // give it a unique name
        population[currentCar].name += "_" + currentCar;
        // Turn off game object until ready to run simulation
        population[currentCar].gameObject.SetActive(false);
        // generate new specimen object for this car for its stats
        currentCarStatistics[currentCar] = new specimen();
    }

    private void runSimulation()
    {
        // Car will attempt to go through the track
        
        population[currentCar].gameObject.SetActive(true);
        
        // calculate the fitness score

        //fitnessScores[index] = gatherFitnessScore();
    }
    // to hold parent values
    public specimen parent1;
    public specimen parent2;

    public void generateNextGeneration() // star trek style...AKA Reproduction cycle
    {
        specimen currentCarStats;
        // replace the current generation
        Debug.Log("New Generation:");
        for (int i = 0; i < populationSize; i++)
        {
            Debug.Log("New Car " + i);
            selectTwoParents();
            currentCarStats = crossOver();
            
            mutation(ref currentCarStats);
            
            buildNewCar(ref currentCarStats, i);
            // set back to start

            population[i].transform.position = population[i].GetComponent<carStatistics>().startLocation;
            
            // now place wheels back to spots
            //foreach (Transform child in population[i].transform) // set all children back to 0,0,0
            //{
            //    child.transform.localPosition = new Vector3(0, 0, 0);
            //}
            // set genOne flag to false
            // then place into an instantiated car object
            // place into populationArray
        }
    }
    public void selectTwoParents()
    {
        // pick a number from the hat
        int parentIndex1 = Random.Range(0, matingPool.Count - 1);
        int parentIndex2 = Random.Range(0, matingPool.Count - 1);

        parent1 = matingPool[parentIndex1]; // grab the parent from the mating pool
        parent2 = matingPool[parentIndex2]; // grab the parent from the mating pool

    }
    private specimen crossOver()
    {
        specimen child = new specimen();
        // randomly slect between the parents for each trait
        // Torque
        int rng = Random.Range(0, 2048); // same for each wheel
        child.torque =  (rng % 2 == 0) ? parent1.torque : parent2.torque;
        // Speed - aka - targetVelocity
        rng = Random.Range(0, 2048); // ssame for each wheel
        child.speed = (rng % 2 == 0) ? parent1.speed : parent2.speed;
        // weight of frame
        rng = Random.Range(0, 2048);
        child.weight = (rng % 2 == 0) ? parent1.weight : parent2.weight;
        // numOfWheels
        // exchange wheel map and wheel dims
        rng = Random.Range(0, 2048);
        if (rng % 2 == 0) // parent 1 
        {
            child.wheelMap = parent1.wheelMap;
            child.wheelDims = parent1.wheelDims;
            child.numOfWheels = parent1.numOfWheels;
            
        }
        else // parent 2
        {
            child.wheelMap = parent2.wheelMap;
            child.wheelDims = parent2.wheelDims;
            child.numOfWheels = parent2.numOfWheels;
        }

        return child;
    }
    private void mutation(ref specimen currentCarStats)
    {
        int randomMuation = Random.Range(0, 100);
        // torque
        if(mutationRate > randomMuation)
        {
            currentCarStats.torque = Random.Range(1, 100);
        }
        // speed
        randomMuation = Random.Range(0, 100);
        if (mutationRate > randomMuation)
        {
            currentCarStats.speed = Random.Range(1, 5000);
        }
        // weight
        randomMuation = Random.Range(0, 100);
        if (mutationRate > randomMuation)
        {
            currentCarStats.weight = Random.Range(1, 200);
        }
        // wheels
        randomMuation = Random.Range(0, 100);
        if (mutationRate > randomMuation)
        {
            currentCarStats.numOfWheels = 0;
            Debug.Log("MUTATING!!!");
            for(int i = 0; i < 4; i++)
            {
                currentCarStats.wheelMap[i] = (Random.Range(0, 100) % 2 == 0) ? true : false;
                if(currentCarStats.wheelMap[i] == true)
                {
                    currentCarStats.numOfWheels++;
                    currentCarStats.wheelDims[i] = new Vector3(Random.Range(0.5f, 1f), 0.03f, Random.Range(0.5f, 1f));
                }else
                {
                        if(currentCarStats.numOfWheels > 0)
                        {
                        currentCarStats.numOfWheels--;
                        }
                }
            }
          }
    }
    // replace each population[] stats with new stats
    public void buildNewCar(ref specimen currentCarStats, int index)
    {
        // Reset Frame bits to start position
        population[index].transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0); // reset FRAME PARENT position to 0, 0, 0 locally
        population[index].transform.GetChild(0).transform.GetChild(0).rotation = Quaternion.identity; // set frame OBJ rotation back to 0, 0, 0 locally to FRAME PARENT
        population[index].transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0); // set frame OBJ to 0, 0, 0 locally to FRAME PARENT
        // Set stats of new car
        population[index].gameObject.GetComponent<carStatistics>().velocity = currentCarStats.speed;
        population[index].gameObject.GetComponent<carStatistics>().torque = currentCarStats.torque;
        population[index].gameObject.GetComponent<carStatistics>().instFrame.transform.GetChild(0).GetComponent<Rigidbody>().mass = currentCarStats.weight;
        // eventually do color as part of genetics
        population[index].gameObject.GetComponent<carStatistics>().wheelCount = currentCarStats.numOfWheels;
        if(population[index].gameObject.GetComponent<carStatistics>().wheelCount > 0)
        {
            // first remove all tires that are there
            foreach (Transform child in population[index].transform)
            {
                if(child.tag != "frame") // destroy old wheels if not the frame object
                {
                    Destroy(child.gameObject);
                }
            }
            int j = 0; // go through each wheel index
            foreach (Transform child in population[index].transform)
            {
                foreach (Transform grandchild in child.transform)
                {
                    if (currentCarStats.wheelMap[j])
                    {
                        grandchild.gameObject.SetActive(true);
                        // leave wheel map index to false -- increase interate variable
                        j++;// increment to next tiremap index
                    }
                    else
                    {
                        j++;// increment to next tiremap index
                    }
                }
            }
            // place new wheels on car
            j = 0;
            bool noWheels = true;
            foreach(Transform child in population[index].GetComponent<carStatistics>().instFrame.transform) // 
            {
                foreach (Transform grandchild in child)
                {
                    if (grandchild.gameObject.activeSelf)
                    { // is wheel-placeholder ON/ENABLED
                        noWheels = false; // to flag in the Update() function to allow to pass through the magnitude threshold
                        population[index].GetComponent<carStatistics>().instWheel = Instantiate(population[index].GetComponent<carStatistics>().wheelObj, grandchild.gameObject.transform.position, Quaternion.identity);
                        population[index].GetComponent<carStatistics>().instWheel.transform.localScale = currentCarStats.wheelDims[j];
                        // if for some reason a wheel got assigned an empty Vector3
                        if(population[index].GetComponent<carStatistics>().instWheel.transform.localScale == new Vector3(0, 0, 0))
                        {
                            population[index].GetComponent<carStatistics>().instWheel.transform.localScale= new Vector3(Random.Range(0.5f, 1f), 0.03f, Random.Range(0.5f, 1f));
                            Debug.Log("hit fail safe 0x014ffda");
                        }
                        population[index].GetComponent<carStatistics>().instWheel.transform.Rotate(0, 180, 90); // flip wheel to correct axis
                        population[index].GetComponent<carStatistics>().instWheel.GetComponent<HingeJoint>().connectedBody = population[index].GetComponent<carStatistics>().instFrame.transform.GetChild(0).GetComponent<Rigidbody>();
                        //Set parent
                        population[index].GetComponent<carStatistics>().instWheel.transform.parent = population[index].transform;
                        
                        HingeJoint hinge = population[index].transform.GetComponent<carStatistics>().instWheel.GetComponent<HingeJoint>();
                        JointMotor motor = hinge.motor;
                        motor.force = population[index].gameObject.GetComponent<carStatistics>().torque;
                        motor.targetVelocity = population[index].gameObject.GetComponent<carStatistics>().velocity;
                        j++; // increment to next tiremap index

                    }else
                    {
                        j++;// increment to next tiremap index
                    }
                }
            }
            if (noWheels)
            {
                population[index].GetComponent<carStatistics>().wheelCount = 0;
            }
        }
    }
    private GameObject generateCar()
    {
       // this generates a random car assembly -- use a flag to indicate at start if this is first gen or not
        return Instantiate(car, start.transform.localPosition, Quaternion.identity);
    }
}
