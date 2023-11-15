// agent will pause at a waypoint for a certain amount of time before moving to the next one

using UnityEngine;
using UnityEngine.AI;

public class Infinite : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints for the agent to follow
    private NavMeshAgent agent;
    private int currentWaypointIndex;
    private bool isWaiting;
    private float waitTimer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentWaypointIndex = 0;
        isWaiting = false;
        waitTimer = 0f;

        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned to script!");
            enabled = false;
        }
        else
        {
            // Start moving the agent initially
            MoveToNextWaypoint();
        }
    }

    private void Update()
    {
        if (isWaiting)
        {
            // Increment the wait timer
            waitTimer += Time.deltaTime;

            // Check if the waiting time is over
            if (waitTimer >= 3f) // modify the number of seconds the agent waits with the waitTimer variable
            {
                // Resume moving
                MoveToNextWaypoint();
                isWaiting = false;
            }
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Agent has reached the current waypoint, stop and start waiting
            agent.isStopped = true;
            isWaiting = true;
            waitTimer = 0f;
        }
    }

    private void MoveToNextWaypoint()
    {
        // Increment the waypoint index and wrap around if necessary
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        // Move to the next waypoint
        Vector3 nextWaypoint = waypoints[currentWaypointIndex].position;
        agent.SetDestination(nextWaypoint);

        // Resume movement
        agent.isStopped = false;
    }
}