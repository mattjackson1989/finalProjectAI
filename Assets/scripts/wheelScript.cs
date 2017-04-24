using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelScript : MonoBehaviour {
    private void OnCollisionEnter(Collision collision)
    {
        gameEngine.hasStarted = true; // touches something means the game has started
    }
}
