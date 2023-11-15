// This script uses the Instantiate function to spawn a Gameobject when the user enters Play mode.
// Use this script to spawn agents when play mode is entered.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAtRuntime : MonoBehaviour
{
    public GameObject spawnobject;

    void Start()
    {
        Instantiate(spawnobject);
    }

    void Update()
    {

    }
}
