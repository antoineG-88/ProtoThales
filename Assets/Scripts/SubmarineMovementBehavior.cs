using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMovementBehavior : MonoBehaviour
{
    public SubmarineCounterMeasures submarineCounterMeasuresScript;

    [Header("Submarine Movement")]
    public float submarineSpeed;
    public float submarineSpeedDeepWater;
    private float currentSpeed;
    public int submarineWaypoints;

    [Space]
    public List<Transform> allWaypoints;

    [Space]
    public Transform nextPosition;

    private bool lureIsCreateFlag;
    private int random;
    private float timer;
    private int countWaypointsAchieved = 0;
    [HideInInspector] public bool waypointHacked;
    [HideInInspector] public Vector3 currentDirection;

    private TerrainZone submarineZone;

    private void Start()
    {
        PickRandomWaypoint();
    }

    private void Update()
    {
        if (!submarineCounterMeasuresScript.decoyAreMoving)
        {
            lureIsCreateFlag = false;
            MoveSubmarine();
        }
        else
        {
            //Change waypoint target if creation lure is lauch when submarine is hacking waypoint
            if (timer > 0 && !lureIsCreateFlag)
            {
                lureIsCreateFlag = true;
                timer = 0;
                PickRandomWaypoint();
            }
        }

        ChangeSpeedByZone();
    }

    private void ChangeSpeedByZone()
    {
        submarineZone = TerrainZoneHandler.GetCurrentZone(SeaCoord.Planify(transform.position));
        
        if (submarineZone.relief == TerrainZone.Relief.Hilly)
        {
            currentSpeed = submarineSpeedDeepWater;
        }
        else
        {
            currentSpeed = submarineSpeed;
        }

    }

    private void PickRandomWaypoint()
    {
        random = Random.Range(0, (allWaypoints.Count));
        nextPosition = allWaypoints[random];
        //allWaypoints.RemoveAt(random);
        currentDirection = -(transform.position - nextPosition.position).normalized;
    }

    private void MoveSubmarine()
    {
        if (countWaypointsAchieved < submarineWaypoints)
        {
            if (transform.position == nextPosition.position)
            {
                timer += Time.deltaTime;               

                if (timer >= nextPosition.GetComponent<Waypoints>().hackingTime)
                {
                    allWaypoints.RemoveAt(random);
                    countWaypointsAchieved++;
                    PickRandomWaypoint();
                    waypointHacked = true;
                }
            }
            else
            {
                waypointHacked = false;

                timer = 0;

                transform.position = Vector3.MoveTowards(transform.position, nextPosition.position, Time.deltaTime * currentSpeed);
            }
        }       
    }
}
