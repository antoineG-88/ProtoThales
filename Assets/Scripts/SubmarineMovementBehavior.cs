using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMovementBehavior : MonoBehaviour
{
    [Header("Submarine Movement")]
    public float submarineSpeed;
    public int submarineWaypoints;
    public float waitTimeAtPoint;

    [Space]
    public List<Transform> allWaypoints;

    [Space]
    public Transform nextPosition;

    private float timer;
    private int countWaypointsAchieved = 0;
    [HideInInspector] public bool waypointHacked;

    private void Start()
    {
        PickRandomWaypoint();
    }

    private void Update()
    {
        MoveSubmarine();
    }

    private void PickRandomWaypoint()
    {
        int random = Random.Range(0, (allWaypoints.Count));
        nextPosition = allWaypoints[random];
        allWaypoints.RemoveAt(random);
    }

    private void MoveSubmarine()
    {
        if (countWaypointsAchieved < submarineWaypoints)
        {
            if (transform.position == nextPosition.position)
            {
                timer += Time.deltaTime;               

                if (timer >= waitTimeAtPoint)
                {
                    countWaypointsAchieved++;
                    PickRandomWaypoint();
                    waypointHacked = true;
                }
            }
            else
            {
                waypointHacked = false;

                timer = 0;

                transform.position = Vector3.MoveTowards(transform.position, nextPosition.position, Time.fixedDeltaTime * submarineSpeed);
            }
        }       
    }
}
