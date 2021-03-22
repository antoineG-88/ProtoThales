using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMovementBehavior : MonoBehaviour
{
    public SubmarineCounterMeasures submarineCounterMeasuresScript;
    public FregateMovement fregateMovement;
    public HullSonarBehavior hullSonarBehavior;
    [Header("Submarine Movement")]
    public float submarineSpeed;
    public float submarineSpeedCoast;
    private float currentSpeed;
    public int submarineWaypoints;
    public float zoneDetectionDistance;
    public float zoneDetectionSteps;
    public float deviationAngle;
    public float angleDetectionStep;
    public float turnSpeed;

    [Space]
    public List<Waypoints> allWaypoints;
    public List<Transform> spawnPoints;

    [Space]
    public Waypoints nextWaypoint;

    [HideInInspector] public Vector2 currentDirection;
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
    [HideInInspector] public LayerMask avoidanceLayerMask;

    [HideInInspector] public TerrainZone submarineZone;

    private List<Vector3> iconGizmos = new List<Vector3>();
    private bool wasLandOnLeft;
    private bool wasLandOnRight;
    private bool landOnRight;
    private bool landOnLeft;
    private bool isAvoidingFregate;

    private void Start()
    {
        PickRandomWaypoint();
        currentPosition = SeaCoord.Planify(spawnPoints[Random.Range(0, spawnPoints.Count)].position);
    }

    private void Update()
    {
        iconGizmos.Clear();

        destinationDirection = SeaCoord.Planify(nextWaypoint.transform.position) - currentPosition;
        destinationDirection.Normalize();
        if (!submarineCounterMeasuresScript.decoyAreMoving)
        {
            lureIsCreateFlag = false;
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
        MoveSubmarine();
    }

    private void ChangeSpeedByZone()
    {
        submarineZone = TerrainZoneHandler.GetCurrentZone(currentPosition, submarineZone);

        if (submarineZone != null && submarineZone.relief == TerrainZone.Relief.Coast)
        {
            currentSpeed = submarineSpeedCoast;
        }
        else
        {
            currentSpeed = submarineSpeed;
        }

    }

    public void PickRandomWaypoint()
    {
        if (allWaypoints.Count > 0)
        {
            random = Random.Range(0, (allWaypoints.Count));
            nextWaypoint = allWaypoints[random];
            //dirtyCurrentDirection = -(transform.position - nextWaypoint.transform.position).normalized;
        }
        else
        {
            Debug.LogWarning("Can't pick another waypoint");
        }
    }

    private float inclinaison;

    private void MoveSubmarine()
    {
        if (countWaypointsAchieved < submarineWaypoints)
        {
            if (Vector2.Distance(SeaCoord.Planify(transform.position), SeaCoord.Planify(nextWaypoint.transform.position)) < nextWaypoint.hackingDistance && !isAvoidingFregate)
            {
                timer += Time.deltaTime;
                submarineCounterMeasuresScript.canAvoidFregate = false;

                if (timer >= nextWaypoint.hackingTime)
                {
                    if (allWaypoints.Count > 0)
                    {
                        allWaypoints.RemoveAt(random);
                        countWaypointsAchieved++;
                        PickRandomWaypoint();
                        waypointHacked = true;
                    }
                    else
                    {
                        Debug.LogWarning("No more waypoint available");
                    }
                }
            }
            else
            {
                if (submarineZone.relief == TerrainZone.Relief.Land)
                {
                    currentDestDirection = destinationDirection;
                }
                else if (IsLandInFront(currentDestDirection) || wasLandOnLeft || wasLandOnRight)
                {
                    AvoidLandMovement();
                }
                else if (avoidanceLayerMask != 0)
                {
                    AvoidFregateMovement();
                }
                else
                {
                    isAvoidingFregate = false;
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

                currentPosition += currentDirection * currentSpeed * Time.deltaTime;
                transform.position = SeaCoord.GetFlatCoord(currentPosition);
            }
        }
    }

    Vector2 leftDirection;
    Vector2 rightDirection;
    private void AvoidLandMovement()
    {
        if (IsLandInFront(currentDestDirection))
        {
            inclinaison = deviationAngle;
            do
            {
                leftDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) + inclinaison);
                rightDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) - inclinaison);

                landOnLeft = IsLandInFront(leftDirection);
                landOnRight = IsLandInFront(rightDirection);


                if (landOnRight && !landOnLeft)
                {
                    wasLandOnRight = true;
                    currentDestDirection = leftDirection;
                }
                else if (landOnLeft && !landOnRight)
                {
                    wasLandOnLeft = true;
                    currentDestDirection = rightDirection;
                }
                else if (!landOnLeft && !landOnRight)
                {
                    //currentDestDirection = rightDirection;
                }

                inclinaison += angleDetectionStep;

            } while (landOnLeft && landOnRight && inclinaison <= 180);
        }
        else
        {
            if (wasLandOnRight)
            {
                rightDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) - inclinaison);
                if (!IsLandInFront(rightDirection))
                {
                    currentDestDirection = rightDirection;
                }
            }

            if (wasLandOnLeft)
            {
                leftDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) + inclinaison);
                if (!IsLandInFront(leftDirection))
                {
                    currentDestDirection = leftDirection;
                }
            }

            if (!IsLandInFront(destinationDirection))
            {
                wasLandOnLeft = false;
                wasLandOnRight = false;
            }
        }
    }

    private bool wasFregateOnLeft;
    private bool wasFregateOnRight;

    private void AvoidFregateMovement()
    {
        if (Vector2.Distance(currentPosition, fregateMovement.currentPosition) < hullSonarBehavior.sonarDistanceDetection)
        {
            isAvoidingFregate = true;
            currentDestDirection = currentPosition - fregateMovement.currentPosition;
            currentDestDirection.Normalize();
        }
        /*else if (IsFregateHullSonarInFront(currentDestDirection) || wasFregateOnLeft || wasFregateOnRight)
        {
            isAvoidingFregate = false;
            if (IsFregateHullSonarInFront(currentDestDirection))
            {
                inclinaison = deviationAngle;
                do
                {
                    leftDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) + inclinaison);
                    rightDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) - inclinaison);

                    landOnLeft = IsFregateHullSonarInFront(leftDirection);
                    landOnRight = IsFregateHullSonarInFront(rightDirection);


                    if (landOnRight && !landOnLeft)
                    {
                        wasFregateOnRight = true;
                        currentDestDirection = leftDirection;
                    }
                    else if (landOnLeft && !landOnRight)
                    {
                        wasFregateOnLeft = true;
                        currentDestDirection = rightDirection;
                    }
                    else if (!landOnLeft && !landOnRight)
                    {
                        //currentDestDirection = rightDirection;
                    }

                    inclinaison += angleDetectionStep;

                } while (landOnLeft && landOnRight && inclinaison <= 180);
            }
            else
            {
                if (wasFregateOnRight)
                {
                    rightDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) - inclinaison);
                    if (!IsFregateHullSonarInFront(rightDirection))
                    {
                        currentDestDirection = rightDirection;
                    }
                }

                if (wasFregateOnLeft)
                {
                    leftDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDestDirection) + inclinaison);
                    if (!IsFregateHullSonarInFront(leftDirection))
                    {
                        currentDestDirection = leftDirection;
                    }
                }

                if (Vector2.Angle(currentDestDirection, destinationDirection) < angleDetectionStep + 1)
                {
                    wasFregateOnLeft = false;
                    wasFregateOnRight = false;
                }
            }
        }*/
        else
        {
            isAvoidingFregate = false;
            currentDestDirection = destinationDirection;
        }
    }

    private bool IsLandInFront(Vector2 direction)
    {
        TerrainZone zone = null;
        bool isLandInFront = false;
        for (int i = 1; i <= zoneDetectionSteps; i++)
        {
            zone = TerrainZoneHandler.GetCurrentZone(currentPosition + direction * i * (Mathf.Min(Vector2.Distance(currentPosition, SeaCoord.Planify(nextWaypoint.transform.position)), zoneDetectionDistance) / zoneDetectionSteps), null);

            //iconGizmos.Add(SeaCoord.GetFlatCoord(currentPosition + direction * i * (Mathf.Min(Vector2.Distance(currentPosition, SeaCoord.Planify(nextWaypoint.transform.position)), zoneDetectionDistance) / zoneDetectionSteps)));

            if ((zone != null && zone.relief == TerrainZone.Relief.Land) || zone == null)
            {
                isLandInFront = true;
            }
        }
        return isLandInFront;
    }

    private bool IsFregateHullSonarInFront(Vector2 direction)
    {
        bool isFregateInFront = false;
        for (int i = 1; i <= zoneDetectionSteps; i++)
        {
            if (Vector2.Distance(currentPosition + direction * i * zoneDetectionDistance / zoneDetectionSteps, fregateMovement.currentPosition) < hullSonarBehavior.sonarDistanceDetection)
            {
                isFregateInFront = true;
            }
        }
        return isFregateInFront;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < iconGizmos.Count; i++)
        {
            Gizmos.DrawIcon(iconGizmos[i], "sv_icon_dot13_pix16_gizmo");
        }
    }
}