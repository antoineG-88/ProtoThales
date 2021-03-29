using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMoveHandler : MonoBehaviour
{
    public SubmarineActionHandler submarineActionHandler;
    public SubmarineHackingBehavior submarineHackingBehavior;
    public FregateMovement fregateMovement;
    [Header("Submarine Movement")]
    public float submarineSpeed;
    private float currentSpeed;
    public float zoneDetectionDistance;
    public float zoneDetectionSteps;
    public float deviationAngle;
    public float angleDetectionStep;
    public float turnSpeed;
    [Header("Submarine Path Behavior")]
    public int subZone12Subdivision;
    public int subZone3Subdivision;
    public float subZoneDetectionPointDistance;

    [Space]
    public List<Waypoints> allWaypoints;
    public List<Transform> spawnPoints;

    [Space]
    [HideInInspector] public Waypoints nextWaypoint;

    [HideInInspector] public Vector2 currentDirection;
    private float currentTurnSide;
    private float currentAngle;
    private Vector2 currentPosition;
    private Vector2 currentDestDirection;
    private Vector2 destinationDirection;
    private Vector2 intermediateDirection;
    private Vector2 nextIntermediatePosition;
    private bool lureIsCreateFlag;
    private int random;
    [HideInInspector] public LayerMask avoidanceLayerMask;
    public LayerMask fregateLayerMask;

    [HideInInspector] public TerrainZone submarineZone;

    private List<Vector3> iconGizmos = new List<Vector3>();
    private bool wasLandOnLeft;
    private bool wasLandOnRight;
    private bool landOnRight;
    private bool landOnLeft;
    private bool isAvoidingFregate;
    private bool isSubmarineDisplayed;

    void Start()
    {
        PickRandomWaypoint();
        currentPosition = SeaCoord.Planify(spawnPoints[Random.Range(0, spawnPoints.Count)].position);
        currentSpeed = submarineSpeed;
    }

    void Update()
    {
        iconGizmos.Clear();
        UpdateZone();
        if (!submarineActionHandler.decoyAreMoving)
        {
            lureIsCreateFlag = false;
        }
        else
        {
            //Change waypoint target if creation lure is lauch when submarine is hacking waypoint
            if (nextWaypoint.currentHackedTime > 0 && !lureIsCreateFlag)
            {
                lureIsCreateFlag = true;
                nextWaypoint.currentHackedTime = 0;
                PickRandomWaypoint();
            }
        }
        MoveSubmarine();
    }

    private void UpdateZone()
    {
        submarineZone = TerrainZoneHandler.GetCurrentZone(currentPosition, submarineZone);
    }

    private void MoveSubmarine()
    {
        if (submarineHackingBehavior.currentWaypointsHacked < allWaypoints.Count && nextWaypoint != null)
        {
            destinationDirection = SeaCoord.Planify(nextWaypoint.transform.position) - currentPosition;
            destinationDirection.Normalize();
            if (Vector2.Distance(SeaCoord.Planify(transform.position), SeaCoord.Planify(nextWaypoint.transform.position)) < nextWaypoint.hackingDistance && !isAvoidingFregate)
            {
                nextWaypoint.currentHackedTime += Time.deltaTime;

                if (nextWaypoint.currentHackedTime >= nextWaypoint.hackingTime)
                {
                    PickRandomWaypoint();
                    submarineHackingBehavior.FinishHack();
                    nextWaypoint.isHacked = true;
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
                    avoidanceLayerMask = fregateLayerMask;
                }
                else
                {
                    isAvoidingFregate = false;
                    currentDestDirection = destinationDirection;

                    if((currentPosition - nextIntermediatePosition).magnitude < 0.1f)
                    {
                        nextIntermediatePosition = FindNextIntermediatePosition();
                    }
                    FindNextIntermediatePosition(); // juste pour debug
                    intermediateDirection = nextIntermediatePosition - currentPosition;
                    intermediateDirection.Normalize();

                    currentDestDirection = intermediateDirection;
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

                currentPosition += currentDirection * currentSpeed * Time.deltaTime;
                transform.position = SeaCoord.GetFlatCoord(currentPosition);
            }
        }
    }

    private Vector2 FindNextIntermediatePosition()
    {



        return new Vector2();
    }

    private float GetSubZoneWeight(float minAngle, float maxAngle, float minRange, float maxRange)
    {
        return 0;
    }

    public void PickRandomWaypoint()
    {
        bool[] hackedWaypoint = new bool[allWaypoints.Count];
        int numberOfWaypointsHacked = 0;
        int rand;
        bool chosenWaypointIsAlreadyHacked;
        do
        {
            chosenWaypointIsAlreadyHacked = false;
            rand = Random.Range(0, allWaypoints.Count);
            if (allWaypoints[rand].isHacked)
            {
                chosenWaypointIsAlreadyHacked = true;
                if(!hackedWaypoint[rand])
                {
                    hackedWaypoint[rand] = true;
                    numberOfWaypointsHacked++;
                }
            }
        } while (chosenWaypointIsAlreadyHacked && numberOfWaypointsHacked < allWaypoints.Count);


        if(numberOfWaypointsHacked < allWaypoints.Count)
        {
            nextWaypoint = allWaypoints[rand];
        }
        else
        {
            Debug.LogWarning("Can't pick another waypoint");
        }
    }

    float inclinaison;
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
    public void DisplaySubmarine(bool selected)
    {
        isSubmarineDisplayed = selected;
        submarineActionHandler.rangeDisplay.SetActive(isSubmarineDisplayed);
    }
}
