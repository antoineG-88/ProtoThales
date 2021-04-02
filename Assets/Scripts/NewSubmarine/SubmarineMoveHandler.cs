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
    public float minRange;
    public int subZone12Subdivision;
    public int subZone3SubSubdivision;
    public float subZoneDetectionPointDistance;
    public List<Transform> beneficialPointFactors;
    public int avoidEffectSliceReach;
    public float intermediatePosRefreshRate;
    public float distanceToRefrehIntemediatePos;
    public float benefPointFactorWeightWhileCalme, benefPointFactorWeightWhileInquiet, benefPointFactorWeightWhilePanique;
    public float distanceFactorWeightWhileCalme, distanceFactorWeightWhileInquiet, distanceFactorWeightWhilePanique;
    float subZoneAngleWidth12;
    float subZoneAngleWidth3;
    private float timeBeforeNextRefresh;

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
        DisplaySubmarine(false);
        PickRandomWaypoint();
        currentPosition = SeaCoord.Planify(spawnPoints[Random.Range(0, spawnPoints.Count)].position);
        nextIntermediatePosition = currentPosition;
        currentSpeed = submarineSpeed;
        subZoneAngleWidth12 = 360 / subZone12Subdivision;
        subZoneAngleWidth3 = 360 / (subZone12Subdivision * subZone3SubSubdivision);
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
            if (Vector2.Distance(currentPosition, nextIntermediatePosition) < distanceToRefrehIntemediatePos)
            {
                RefreshIntermediatePosition();
            }

            if (timeBeforeNextRefresh > 0)
            {
                timeBeforeNextRefresh -= Time.deltaTime;
            }
            else
            {
                RefreshIntermediatePosition();
                timeBeforeNextRefresh = intermediatePosRefreshRate;
            }

            FindNextIntermediatePosition();
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

    private void RefreshIntermediatePosition()
    {
        nextIntermediatePosition = FindNextIntermediatePosition();
        intermediateDirection = nextIntermediatePosition - currentPosition;
        intermediateDirection.Normalize();
    }

    private List<SubZone> allSubZones = new List<SubZone>();

    private Vector2 FindNextIntermediatePosition()
    {
        allSubZones.Clear();
        float startAngle = Vector2.SignedAngle(Vector2.right, destinationDirection) - (360 / (subZone12Subdivision * subZone3SubSubdivision));
        for (int i = 0; i < subZone12Subdivision; i++)
        {
            allSubZones.Add(new SubZone(GetNormAngle(startAngle + i * subZoneAngleWidth12), GetNormAngle(startAngle + (i + 1) * subZoneAngleWidth12), minRange, submarineActionHandler.detectionRangeCalme, i, "SZ_" + i + "_0", this));
            if (isSubmarineDisplayed)
                Debug.DrawRay(SeaCoord.GetFlatCoord(currentPosition),
                    SeaCoord.GetFlatCoord(SeaCoord.GetDirectionFromAngle(allSubZones[allSubZones.Count - 1].minAngle) * submarineActionHandler.detectionRangeCalme),
                    Color.green);

            if (submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Inquiet
        || submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Panique)
            {
                allSubZones.Add(new SubZone(GetNormAngle(startAngle + i * subZoneAngleWidth12), GetNormAngle(startAngle + (i + 1) * subZoneAngleWidth12), submarineActionHandler.detectionRangeCalme, submarineActionHandler.detectionRangeInquiet, i, "SZ_" + i + "_1", this));

                if (isSubmarineDisplayed)
                    Debug.DrawRay(SeaCoord.GetFlatCoord(currentPosition + SeaCoord.GetDirectionFromAngle(allSubZones[allSubZones.Count - 1].minAngle) * submarineActionHandler.detectionRangeCalme),
                        SeaCoord.GetFlatCoord(SeaCoord.GetDirectionFromAngle(allSubZones[allSubZones.Count - 1].minAngle) * (submarineActionHandler.detectionRangeInquiet - submarineActionHandler.detectionRangeCalme)),
                        Color.cyan);


                if (submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Panique)
                {
                    for (int y = 0; y < subZone3SubSubdivision; y++)
                    {
                        allSubZones.Add(new SubZone(GetNormAngle(startAngle + i * subZoneAngleWidth12 + y * subZoneAngleWidth3), GetNormAngle(startAngle + i * subZoneAngleWidth12 + (y + 1) * subZoneAngleWidth3), submarineActionHandler.detectionRangeInquiet, submarineActionHandler.detectionRangePanique, i, "SZ_" + i + "_2." + y, this));
                        if (isSubmarineDisplayed)
                            Debug.DrawRay(SeaCoord.GetFlatCoord(currentPosition + SeaCoord.GetDirectionFromAngle(allSubZones[allSubZones.Count - 1].minAngle) * submarineActionHandler.detectionRangeInquiet), SeaCoord.GetFlatCoord(SeaCoord.GetDirectionFromAngle(allSubZones[allSubZones.Count - 1].minAngle) * (submarineActionHandler.detectionRangePanique - submarineActionHandler.detectionRangeInquiet)), Color.red);
                    }
                }
            }
            /*if (isSubmarineDisplayed)
                circleGismos.Add(new CircleGizmo(currentPosition, submarineActionHandler.detectionRangeCalme, Color.green));*/

            if (submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Inquiet
            || submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Panique)
            {
                /*if (isSubmarineDisplayed)
                    circleGismos.Add(new CircleGizmo(currentPosition, submarineActionHandler.detectionRangeInquiet, Color.cyan));*/


                if (submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Panique)
                {
                    /*if (isSubmarineDisplayed)
                        circleGismos.Add(new CircleGizmo(currentPosition, submarineActionHandler.detectionRangePanique, Color.red));*/
                }
            }
        }
        SubZone bestSubZone = allSubZones[0];

        for (int i = 0; i < allSubZones.Count; i++)
        {
            allSubZones[i].weight = GetSubZoneWeight(allSubZones[i]);
        }

        for (int i = 0; i < allSubZones.Count; i++)
        {
            if(allSubZones[i].needToBeAvoided && avoidEffectSliceReach > 0)
            {
                for (int y = 0; y < allSubZones.Count; y++)
                {
                    if (allSubZones[y].sliceIndex == allSubZones[i].sliceIndex)
                    {
                        allSubZones[y].weight = -1000;
                        //sphereGizmos.Add(new SphereGizmo(allSubZones[y].zoneCenterPos, 0.4f, Color.black));
                    }

                    for (int s = 1; s < avoidEffectSliceReach; s++)
                    {
                        if ((allSubZones[i].sliceIndex + s) < subZone12Subdivision)
                        {
                            if (allSubZones[y].sliceIndex == (allSubZones[i].sliceIndex + s))
                            {
                                allSubZones[y].weight = -1000;
                                //sphereGizmos.Add(new SphereGizmo(allSubZones[y].zoneCenterPos, 0.4f, Color.black));
                            }
                        }
                        else
                        {
                            if (allSubZones[y].sliceIndex == s - (subZone12Subdivision - allSubZones[i].sliceIndex))
                            {
                                allSubZones[y].weight = -1000;
                                //sphereGizmos.Add(new SphereGizmo(allSubZones[y].zoneCenterPos, 0.4f, Color.black));
                            }
                        }


                        if ((allSubZones[i].sliceIndex - s) >= 0)
                        {
                            if (allSubZones[y].sliceIndex == (allSubZones[i].sliceIndex - s))
                            {
                                allSubZones[y].weight = -1000;
                                //sphereGizmos.Add(new SphereGizmo(allSubZones[y].zoneCenterPos, 0.4f, Color.black));
                            }
                        }
                        else
                        {
                            if (allSubZones[y].sliceIndex == (subZone12Subdivision - (s - allSubZones[i].sliceIndex)))
                            {
                                allSubZones[y].weight = -1000;
                                //sphereGizmos.Add(new SphereGizmo(allSubZones[y].zoneCenterPos, 0.4f, Color.black));
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < allSubZones.Count; i++)
        {
            //sphereGizmos.Add(new SphereGizmo(allSubZones[i].zoneCenterPos, 0.2f, Color.Lerp(Color.red, Color.yellow, (allSubZones[i].weight + 15) / 20)));

            if (bestSubZone == null || allSubZones[i].weight > bestSubZone.weight)
            {
                bestSubZone = allSubZones[i];
            }
        }

        return bestSubZone.zoneCenterPos;
    }

    float pointDistance;
    float pointRelativeAngle;
    Vector2 subZoneDirection;
    private float GetSubZoneWeight(SubZone subZone)
    {
        float weight = 0;

        subZoneDirection = SeaCoord.GetDirectionFromAngle(GetNormAngle(subZone.minAngle + Mathf.DeltaAngle(subZone.minAngle, subZone.maxAngle) * 0.5f));

        for (int o = 0; o < beneficialPointFactors.Count; o++)
        {
            pointDistance = Vector2.Distance(SeaCoord.Planify(beneficialPointFactors[o].position), currentPosition);
            pointRelativeAngle = Vector2.SignedAngle(Vector2.right, SeaCoord.Planify(beneficialPointFactors[o].position) - currentPosition);
            if (pointDistance < subZone.maxRange && pointDistance >= subZone.minRange && IsBetweenAngle(pointRelativeAngle, subZone.minAngle, subZone.maxAngle))
            {
                weight += submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Calme ? benefPointFactorWeightWhileCalme : 0;
                weight += submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Inquiet ? benefPointFactorWeightWhileInquiet : 0;
                weight += submarineActionHandler.currentState == SubmarineActionHandler.VigilanceState.Panique ? benefPointFactorWeightWhilePanique : 0;
            }
        }

        switch (submarineActionHandler.currentState)
        {
            case SubmarineActionHandler.VigilanceState.Calme:
                weight += Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(destinationDirection, subZoneDirection)) * distanceFactorWeightWhileCalme;
                break;

            case SubmarineActionHandler.VigilanceState.Inquiet:
                weight += Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(destinationDirection, subZoneDirection)) * distanceFactorWeightWhileInquiet;
                break;

            case SubmarineActionHandler.VigilanceState.Panique:
                weight += Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(destinationDirection, subZoneDirection)) * distanceFactorWeightWhilePanique;
                break;
        }

        if (TerrainZoneHandler.GetCurrentZone(subZone.zoneCenterPos, null) != null && TerrainZoneHandler.GetCurrentZone(subZone.zoneCenterPos, null).relief == TerrainZone.Relief.Land)
        {
            weight -= 1000;
        }

        pointDistance = Vector2.Distance(fregateMovement.currentPosition, currentPosition);
        pointRelativeAngle = Vector2.SignedAngle(Vector2.right, fregateMovement.currentPosition - currentPosition);
        if (pointDistance < subZone.maxRange && pointDistance >= subZone.minRange && IsBetweenAngle(pointRelativeAngle, subZone.minAngle, subZone.maxAngle))
        {
            weight = -1000;
            subZone.needToBeAvoided = true;
        }

        for (int b = 0; b < submarineActionHandler.madBehavior.sonobuoys.Count; b++)
        {
            pointDistance = Vector2.Distance(SeaCoord.Planify(submarineActionHandler.madBehavior.sonobuoys[b].transform.position), currentPosition);
            pointRelativeAngle = Vector2.SignedAngle(Vector2.right, SeaCoord.Planify(submarineActionHandler.madBehavior.sonobuoys[b].transform.position) - currentPosition);
            if (pointDistance < subZone.maxRange && pointDistance >= subZone.minRange && IsBetweenAngle(pointRelativeAngle, subZone.minAngle, subZone.maxAngle))
            {
                weight = -1000;
                subZone.needToBeAvoided = true;
            }
        }

        return weight;
    }

    private float GetNormAngle(float angle)
    {
        float newAngle = angle;

        if(angle > 180)
        {
            newAngle = angle - 360;
        }

        if(angle <= -180)
        {
            newAngle = angle + 360;
        }
        return newAngle;
    }

    private bool IsBetweenAngle(float angle, float mininmumAngle, float maximumAngle)
    {
        bool isBetween = false;
        if (mininmumAngle > maximumAngle)
        {
            if (angle >= mininmumAngle || angle < maximumAngle)
            {
                isBetween = true;
            }
        }
        else
        {
            if (angle >= mininmumAngle && angle < maximumAngle)
            {
                isBetween = true;
            }
        }

        //Debug.Log("min : " + mininmumAngle + ", max : " + maximumAngle + ", Angle : " + angle + ". Between : " + isBetween);
        return isBetween;
    }

    public void PickRandomWaypoint()
    {
        bool[] hackedWaypoint = new bool[allWaypoints.Count];
        int numberOfWaypointsHacked = 0;
        bool chosenWaypointIsAlreadyHacked;
        do
        {
            chosenWaypointIsAlreadyHacked = false;
            random = Random.Range(0, allWaypoints.Count);
            if (allWaypoints[random].isHacked)
            {
                chosenWaypointIsAlreadyHacked = true;
                if(!hackedWaypoint[random])
                {
                    hackedWaypoint[random] = true;
                    numberOfWaypointsHacked++;
                }
            }
        } while (chosenWaypointIsAlreadyHacked && numberOfWaypointsHacked < allWaypoints.Count);


        if(numberOfWaypointsHacked < allWaypoints.Count)
        {
            nextWaypoint = allWaypoints[random];
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
                nextIntermediatePosition = FindNextIntermediatePosition();
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

    public class SubZone
    {
        public Vector2 zoneCenterPos;
        public float minAngle, maxAngle;
        public float minRange, maxRange;
        public float weight;
        public int sliceIndex;
        public string identity;
        public bool needToBeAvoided;

        public SubZone(float _minAngle, float _maxAngle, float _minRange, float _maxRange, int _sliceIndex, string _identity, SubmarineMoveHandler submarineMoveHandler)
        {
            minAngle = _minAngle;
            maxAngle = _maxAngle;
            minRange = _minRange;
            maxRange = _maxRange;
            sliceIndex = _sliceIndex;
            identity = _identity;
            needToBeAvoided = false;
            weight = -666;

            zoneCenterPos = submarineMoveHandler.currentPosition + SeaCoord.GetDirectionFromAngle(submarineMoveHandler.GetNormAngle(minAngle + Mathf.DeltaAngle(minAngle, maxAngle) * 0.5f)) * (maxRange + minRange) * 0.5f;
        }
    }

    /*private List<CircleGizmo> circleGismos = new List<CircleGizmo>();
    //private List<SphereGizmo> sphereGizmos = new List<SphereGizmo>();
    private void OnDrawGizmos()
    {
        if(isSubmarineDisplayed)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(SeaCoord.GetFlatCoord(nextIntermediatePosition), 0.2f);

            for (int i = 0; i < circleGismos.Count; i++)
            {
                Gizmos.color = circleGismos[i].color;
                Gizmos.DrawWireSphere(SeaCoord.GetFlatCoord(circleGismos[i].seaPosition), circleGismos[i].range);
            }

            for (int i = 0; i < sphereGizmos.Count; i++)
            {
                Gizmos.color = sphereGizmos[i].color;
                Gizmos.DrawSphere(SeaCoord.GetFlatCoord(sphereGizmos[i].seaPosition), sphereGizmos[i].radius);
            }

            circleGismos.Clear();
            sphereGizmos.Clear();
        }
    }

    public class CircleGizmo
    {
        public float range;
        public Vector2 seaPosition;
        public Color color;

        public CircleGizmo(Vector2 pos, float radius, Color _color)
        {
            seaPosition = pos;
            color = _color;
            range = radius;
        }
    }
    public class SphereGizmo
    {
        public float radius;
        public Vector2 seaPosition;
        public Color color;

        public SphereGizmo(Vector2 pos, float _radius, Color _color)
        {
            seaPosition = pos;
            color = _color;
            radius = _radius;
        }
    }*/
}
