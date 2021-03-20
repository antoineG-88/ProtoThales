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
    public float zoneDetectionDistance;
    public float angleDetection;
    public float turnSpeed;

    [Space]
    public List<Waypoints> allWaypoints;

    [Space]
    public Waypoints nextWaypoint;

    private Vector2 currentDirection;
    private float currentTurnSide;
    private float currentAngle;
    private Vector2 currentPosition;
    private Vector2 currentDestDirection;
    private Vector2 destinationDirection;
    private Vector2 curentDirection;
    private bool lureIsCreateFlag;
    private int random;
    private float timer;
    private int countWaypointsAchieved = 0;
    [HideInInspector] public bool waypointHacked;
    [HideInInspector] public Vector3 dirtyCurrentDirection;

    private TerrainZone submarineZone;

    private void Start()
    {
        PickRandomWaypoint();
        currentPosition = SeaCoord.Planify(transform.position);
    }

    private void Update()
    {
        destinationDirection = SeaCoord.Planify(nextWaypoint.transform.position) - currentPosition;
        destinationDirection.Normalize();
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
        submarineZone = TerrainZoneHandler.GetCurrentZone(SeaCoord.Planify(transform.position), submarineZone);
        
        if (submarineZone.relief == TerrainZone.Relief.Hilly)
        {
            currentSpeed = submarineSpeedDeepWater;
        }
        else
        {
            currentSpeed = submarineSpeed;
        }

    }

    public void PickRandomWaypoint()
    {
        random = Random.Range(0, (allWaypoints.Count));
        nextWaypoint = allWaypoints[random];
        //allWaypoints.RemoveAt(random);
        dirtyCurrentDirection = -(transform.position - nextWaypoint.transform.position).normalized;
    }

    private void MoveSubmarine()
    {
        if (!submarineCounterMeasuresScript.raycastTouchObstacle)
        {
            if (countWaypointsAchieved < submarineWaypoints)
            {
                if (Vector2.Distance(SeaCoord.Planify(transform.position), SeaCoord.Planify(nextWaypoint.transform.position)) < nextWaypoint.hackingDistance)
                {
                    timer += Time.deltaTime;
                    submarineCounterMeasuresScript.canAvoidFregate = false;

                    if (timer >= nextWaypoint.hackingTime)
                    {
                        allWaypoints.RemoveAt(random);
                        countWaypointsAchieved++;
                        PickRandomWaypoint();
                        waypointHacked = true;
                    }
                }
                else
                {
                    if (IsLandInFront(currentDestDirection))
                    {
                        Vector2 inclinedDirection1 = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) + angleDetection);
                        if (IsLandInFront(inclinedDirection1))
                        {
                            currentDestDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) - angleDetection);
                        }
                        else
                        {
                            currentDestDirection = inclinedDirection1;
                        }
                    }
                    else
                    {
                        currentDestDirection = destinationDirection;
                    }

                    if (Vector2.Angle(currentDirection, currentDestDirection) > Time.fixedDeltaTime * turnSpeed)
                    {
                        currentTurnSide = Vector2.SignedAngle(currentDirection, currentDestDirection) > 0 ? 1 : -1;
                        currentAngle = Vector2.SignedAngle(Vector2.right, currentDirection) + currentTurnSide * Time.fixedDeltaTime * turnSpeed;
                    }
                    else
                    {
                        currentDirection = currentDestDirection;
                    }
                    currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
                    transform.rotation = SeaCoord.SetRotation(transform.rotation, -currentAngle + 90);

                    waypointHacked = false;

                    timer = 0;

                    //transform.position = Vector3.MoveTowards(transform.position, nextWaypoint.transform.position, Time.deltaTime * currentSpeed);

                    currentPosition += currentDirection * currentSpeed * Time.deltaTime;
                    transform.position = SeaCoord.GetFlatCoord(currentPosition);
                }
            }
        }

        
    }

    private bool IsLandInFront(Vector2 direction)
    {
        TerrainZone zone = null;
        bool isLandInFront = false;
        for (int i = 1; i < 9; i++)
        {
            zone = TerrainZoneHandler.GetCurrentZone(currentPosition + direction * i * (zoneDetectionDistance / 8), null);
            if ((zone != null && zone.relief == TerrainZone.Relief.Land) || zone == null)
            {
                isLandInFront = true;
            }
        }
        return isLandInFront;
    }
}
