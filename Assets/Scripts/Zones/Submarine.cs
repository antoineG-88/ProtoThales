using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    public PinHandler pinHandler;
    public Fregate fregate;
    [Header("Movement")]
    public SubmarinePath path;
    public List<InterestPoint> interestPoints;
    public float pointReachRange;
    public float maxSpeed;
    public float slowSpeed;
    public float acceleration;
    public float turnSpeed;
    public float minDistanceFromEdge;

    [Header("Movement")]
    [Range(1f, 100f)] public float maxVigilance;
    [Range(1f, 100f)] public float alertVigilanceStep;
    public float timeBetweenAlertScans;
    public float alertScanRange;
    public float scanAlertPinRandomOffset;
    public float hideDuration;
    public int hidingVigilanceConsumption;
    public int moveToSafeLocationConsumption;
    [Header("Technical")]
    public float closeZoneSearchRange;
    public int closeZoneSearchAngleInterval;
    public int closeZoneSearchStep;

    private int nextDestIndex;
    [HideInInspector] public Vector2 currentPosition;
    [HideInInspector] public Vector2 currentDirection;
    private float currentAngle;
    private float currentSpeed;
    private Vector2 destinationDirection;
    private Vector2 currentDestination;
    private bool isCustomDestination;
    private int currentTurnSide;
    [HideInInspector] public bool isOnInterestPoint;
    private bool isNextInterestPoint;
    private InterestPoint actualInterestPoint;
    private int actualInterestPointIndex;
    [HideInInspector] public float currentMaxSpeed;

    [HideInInspector] public float vigilance;
    [HideInInspector] public bool isUnderThermocline;
    private float currentCompletionTimeSpend;
    private Zone currentZone;
    private bool isInAlert;
    private float timeRemainingBeforeNextAlertScan;
    private Vector2 closeZonePos;
    private bool alertFlag;
    [HideInInspector] public bool isImmobilized;

    void Start()
    {
        currentMaxSpeed = maxSpeed;
        nextDestIndex = 1;
        currentPosition = SeaCoord.Planify(path.pathPosition[0].position);
        transform.position = SeaCoord.GetFlatCoord(currentPosition);
        currentAngle = 0;
        currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
        actualInterestPointIndex = 0;
        actualInterestPoint = interestPoints[actualInterestPointIndex];
        currentCompletionTimeSpend = 0;
        isImmobilized = false;
    }

    private void Update()
    {
        RefreshDest();

        currentZone = ZoneHandler.GetCurrentZone(currentPosition);
        UpdateVigilance();

        if(Input.touchCount > 3 && InputDuo.tapDown)
        {
            Alert(15);
        }
    }
    private void FixedUpdate()
    {
        UpdateCompletion();
        UpdateMovement();
    }

    private void UpdateBehavior()
    {
        if(isUnderThermocline && currentZone.depth == Zone.Depth.Coast)
        {
            isUnderThermocline = false;
        }
    }

    #region Movement&Path

    private void RefreshDest()
    {
        if (!isCustomDestination)
        {
            if (IsNextInterestPoint())
            {
                currentDestination = SeaCoord.Planify(actualInterestPoint.submarineCompletionLocation.position);
            }
            else
            {
                currentDestination = SeaCoord.Planify(path.pathPosition[nextDestIndex].position);
            }
        }
        destinationDirection = currentDestination - currentPosition;
        isNextInterestPoint = IsNextInterestPoint();
    }

    private void UpdateMovement()
    {
        if(!isImmobilized)
        {
            if ((Vector2.Distance(currentPosition, currentDestination) < pointReachRange && !isNextInterestPoint)
                || isNextInterestPoint && Vector2.Distance(currentPosition, currentDestination) < actualInterestPoint.submarineCompletionRange)
            {
                if (isCustomDestination)
                {
                    if (!isInAlert)
                    {
                        isCustomDestination = false;
                    }


                    if (currentSpeed > 0)
                    {
                        currentSpeed -= acceleration * Time.fixedDeltaTime;
                        if (currentSpeed < 0)
                        {
                            currentSpeed = 0;
                        }
                    }
                }
                else
                {
                    if (isNextInterestPoint)
                    {
                        if (currentSpeed > 0)
                        {
                            currentSpeed -= acceleration * Time.fixedDeltaTime;
                            if (currentSpeed < 0)
                            {
                                currentSpeed = 0;
                            }
                        }

                        isOnInterestPoint = true;
                    }
                    else
                    {
                        if (nextDestIndex < path.pathPosition.Count - 1)
                        {
                            nextDestIndex++;
                            RefreshDest();
                        }
                    }
                }
            }
            else
            {
                if (currentSpeed <= currentMaxSpeed)
                {
                    if (currentSpeed < currentMaxSpeed - acceleration * Time.fixedDeltaTime)
                    {
                        currentSpeed += acceleration * Time.fixedDeltaTime;
                    }
                    else
                    {
                        currentSpeed = currentMaxSpeed;
                    }
                }
                else
                {
                    if (currentSpeed > currentMaxSpeed + acceleration * Time.fixedDeltaTime)
                    {
                        currentSpeed -= acceleration * Time.fixedDeltaTime;
                    }
                    else
                    {
                        currentSpeed = currentMaxSpeed;
                    }
                }

                if (Vector2.Angle(currentDirection, destinationDirection) > Time.fixedDeltaTime * turnSpeed)
                {
                    currentTurnSide = Vector2.SignedAngle(currentDirection, destinationDirection) > 0 ? 1 : -1;
                    currentAngle = Vector2.SignedAngle(Vector2.right, currentDirection) + currentTurnSide * Time.fixedDeltaTime * turnSpeed;
                }
                else
                {
                    currentDirection = destinationDirection;
                }

                currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
                transform.rotation = SeaCoord.SetRotation(transform.rotation, -currentAngle + 90);

                if (isNextInterestPoint)
                {
                    currentCompletionTimeSpend = 0;
                }
                isOnInterestPoint = false;
            }
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= acceleration * Time.fixedDeltaTime;
                if (currentSpeed < 0)
                {
                    currentSpeed = 0;
                }
            }
        }

        MoveForward(currentSpeed);
    }

    private void CompleteActualInterestPoint()
    {
        actualInterestPoint.isComplete = true;
        if (actualInterestPointIndex < interestPoints.Count)
        {
            actualInterestPointIndex++;
            actualInterestPoint = interestPoints[actualInterestPointIndex];
        }

        if (nextDestIndex < path.pathPosition.Count - 1)
        {
            nextDestIndex++;
        }
    }

    private void UpdateCompletion()
    {
        if (isOnInterestPoint)
        {
            if (currentCompletionTimeSpend < actualInterestPoint.submarineCompletionTime)
            {
                currentCompletionTimeSpend += Time.fixedDeltaTime;
            }
            else
            {
                CompleteActualInterestPoint();
            }
        }
    }

    private bool IsNextInterestPoint()
    {
        bool isInterestPoint = false;
        for (int i = 0; i < interestPoints.Count; i++)
        {
            if (interestPoints[i].pathIndex == nextDestIndex)
            {
                isInterestPoint = true;
            }
        }
        return isInterestPoint;
    }

    protected void MoveForward(float speed)
    {
        currentPosition += currentDirection * speed * Time.fixedDeltaTime;
        transform.position = SeaCoord.GetFlatCoord(currentPosition);
    }

    public IEnumerator Immobilize(float time)
    {
        isImmobilized = true;
        yield return new WaitForSeconds(time);
        isImmobilized = false;
    }

    #endregion

    #region Vigilance

    private void UpdateVigilance()
    {
        if (!isInAlert)
        {
            if (vigilance > alertVigilanceStep)
            {
                isInAlert = true;
            }
            alertFlag = true;
        }
        else
        {
            if(alertFlag)
            {
                timeRemainingBeforeNextAlertScan = 0;
                StartAlertScan();
                nextDestIndex = interestPoints[actualInterestPointIndex].pathIndex;
                alertFlag = false;
            }

            if (vigilance <= 0)
            {
                isInAlert = false;
            }

            if (timeRemainingBeforeNextAlertScan > 0)
            {
                timeRemainingBeforeNextAlertScan -= Time.deltaTime;
            }
            else
            {
                if (StartAlertScan())
                {
                    if (currentZone.depth == Zone.Depth.Deep)
                    {
                        Hide();
                    }
                    else
                    {
                        closeZonePos = GetCloseDeepZone();
                        if (closeZonePos != Vector2.zero)
                        {
                            isCustomDestination = true;
                            currentDestination = closeZonePos;
                            UseVigilance(moveToSafeLocationConsumption);
                        }
                        else
                        {
                            closeZonePos = GetCloseStormyZone();
                            if (closeZonePos != Vector2.zero)
                            {
                                isCustomDestination = true;
                                currentDestination = closeZonePos;
                                UseVigilance(moveToSafeLocationConsumption);
                            }
                        }
                    }

                }
                timeRemainingBeforeNextAlertScan = timeBetweenAlertScans;
            }
        }
    }

    public bool StartAlertScan()
    {
        float distanceToFregate = Vector2.Distance(fregate.currentPosition, currentPosition);
        Vector2 randomVector = new Vector2(Random.Range(-scanAlertPinRandomOffset, scanAlertPinRandomOffset), Random.Range(-scanAlertPinRandomOffset, scanAlertPinRandomOffset));
        if (distanceToFregate < alertScanRange)
        {
            pinHandler.CreateScanAlertPin(currentPosition + randomVector);
            return true;
        }
        return false;
    }

    public void Alert(float vigilanceIncrease)
    {
        vigilance += vigilanceIncrease;
        Debug.Log("Vigilance increased to : " + vigilance);
        vigilance = Mathf.Clamp(vigilance, 0, maxVigilance);
    }

    public void UseVigilance(int amount)
    {
        vigilance -= amount;
        vigilance = Mathf.Clamp(vigilance, 0, maxVigilance);
        Debug.Log("Vigilance consumed to : " + vigilance);
    }

    /// <summary>
    /// Return vector2.zero if no pos found
    /// </summary>
    private Vector2 GetCloseDeepZone()
    {
        Vector2 closestDeepZonePos = Vector2.zero;
        Zone zoneFound = null;
        float closestDist = 600;
        if (closeZoneSearchAngleInterval > 0 && closeZoneSearchStep > 0)
        {
            for (int a = 0; a < 360; a += closeZoneSearchAngleInterval)
            {
                Vector2 dir = SeaCoord.GetDirectionFromAngle(a);
                for (int i = 0; i < closeZoneSearchRange; i += closeZoneSearchStep)
                {
                    zoneFound = ZoneHandler.GetCurrentZone(currentPosition + dir * i);
                    if (zoneFound != null && zoneFound.depth == Zone.Depth.Deep)
                    {
                        if (Vector2.Distance(currentPosition + dir * i, currentPosition) < closestDist)
                        {
                            closestDeepZonePos = currentPosition + dir * (i + minDistanceFromEdge);
                            closestDist = Vector2.Distance(currentPosition + dir * i, currentPosition);
                        }
                    }
                }
            }
        }
        return closestDeepZonePos;
    }

    private Vector2 GetCloseStormyZone()
    {
        Vector2 closestStormyZonePos = Vector2.zero;
        Zone zoneFound = null;
        float closestDist = 600;
        if (closeZoneSearchAngleInterval > 0 && closeZoneSearchStep > 0)
        {
            for (int a = 0; a < 360; a += closeZoneSearchAngleInterval)
            {
                Vector2 dir = SeaCoord.GetDirectionFromAngle(a);
                for (int i = 0; i < closeZoneSearchRange; i += closeZoneSearchStep)
                {
                    zoneFound = ZoneHandler.GetCurrentZone(currentPosition + dir * i);
                    if (zoneFound != null && zoneFound.currentWeather == Zone.Weather.Storm)
                    {
                        if (Vector2.Distance(currentPosition + dir * i, currentPosition) < closestDist)
                        {
                            closestStormyZonePos = currentPosition + dir * (i + minDistanceFromEdge);
                            closestDist = Vector2.Distance(currentPosition + dir * i, currentPosition);
                        }
                    }
                }
            }
        }
        return closestStormyZonePos;
    }

    private void Hide()
    {
        UseVigilance(hidingVigilanceConsumption);
        isUnderThermocline = true;
    }

    #endregion


    [System.Serializable]
    public class InterestPoint
    {
        public Transform position;
        public float submarineCompletionTime;
        public Transform submarineCompletionLocation;
        public float submarineCompletionRange;
        public bool isComplete;
        public int pathIndex;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(SeaCoord.GetFlatCoord(currentDestination), 0.8f);
    }
}
