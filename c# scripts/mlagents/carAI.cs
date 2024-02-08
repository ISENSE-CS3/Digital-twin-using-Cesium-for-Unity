//Script for machine learning traffic simulation
//To be applied to each vehicle
//Anton Rajko FAU

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class carAI : Agent
{
    //This displays the destination of the car in the inspector (for testing)
    [SerializeField] private Transform targetTransform;

    //Array of possible destinations of the car (each car with this script attached can have a different set)
    //public Transform[] destination = new Transform[1];
    [SerializeField] private Transform[] destination = new Transform[1];

    //Initializes a position that the car will respawn in after end of episode
    Vector3 startPosition;
    Quaternion startRotation;

    //Reference to the script that handles movement
    private movement movementScript;

    //Initializes physics object for the car
    private Rigidbody rb;

    void Start()
    {

        //Sets the original position of the car as the position where it will respawn after episode ends
        startPosition = transform.position;
        startRotation = transform.rotation;

    }

    private void Awake()
    {
        //Grabs the movement script
        movementScript = GetComponent<movement>();
    }

    public override void Initialize()
    {
        //Assigns physics object
        rb = GetComponent<Rigidbody>();
    }

    //Handles keyboard input for the car: when behavior type is set to "Heuristic" the user can move the car by using the WASD keys (used for demo)
    public override void Heuristic(in ActionBuffers actionsOut)
    {

        int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = vertical >= 0 ? vertical : 2;
        actions[1] = horizontal >= 0 ? horizontal : 2;

    }


    //Occurs at the beginning of each run
    public override void OnEpisodeBegin()
    {
        //Sets all destination options active at the beginning of episode, later all are turned off except one
        for (int i = 0; i < destination.Length; i++)
        {

            destination[i].gameObject.SetActive(true);

        }

        //Car respawns at original location and facing the original direction
        transform.position = startPosition;
        transform.rotation = startRotation;


        //Picks random destination from array of possible destinations in order for cars to have randomized factors when training
        int randomIndex = Random.Range(0, destination.Length);
        Transform randomTransform = destination[randomIndex];
        for (int i = 0; i < destination.Length; i++)
        {

            if (i != randomIndex)

            {

                destination[i].gameObject.SetActive(false);

            }

            else targetTransform = destination[i].transform;
        }


    }

    //Gives the vehicle information about its position as well as the destination's position
    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);

    }

    //Handles movement and rotation of the car
    public override void OnActionReceived(ActionBuffers actions)
    {

        float vertical = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        float horizontal = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;

        //Function from movement.cs that handles movement and rotation
        movementScript.SetInputs(vertical, horizontal);

    }

    //Handles collisions with triggers and adjusts reward and punishment accordingly (checkpoints, walls destinations, traffic lights etc.)
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Goal"))
        {

            AddReward(+15f);
            EndEpisode();
            rb.velocity = Vector3.zero;
            Debug.Log("Destination reached");

        }

        if (other.CompareTag("Wall"))
        {

            AddReward(-1f);
            EndEpisode();
            rb.velocity = Vector3.zero;
            Debug.Log("Wall hit");

        }

        if (other.CompareTag("Checkpoint"))
        {

            SetReward(+0.5f);
            Debug.Log("Checkpoint reached");

        }

        if (other.CompareTag("Red Light"))
        {

            AddReward(-0.5f);
            movementScript.SetInputs(0, 0);
            Debug.Log("Red Light ran");

        }

        if (other.CompareTag("Bubble")) //Each car has a safety bubble besides the main collider to indicate safe spacing between vehicles
        {

            AddReward(-2.5f);
            Debug.Log("Safety bubble breached");

        }

    }

    //Handles collisions with non-trigger objects such as other cars, buildings, pedestrians etc.
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Car"))
        {

            AddReward(-5f);
            Debug.Log("Car crash");

        }

    }

}
