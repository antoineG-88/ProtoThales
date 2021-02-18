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

    [Header("Movement")]
    [Range(1f, 100f)] public float maxVigilance;
    [Range(1f, 100f)] public float alertVigilanceStep;
    public float timeBetweenAlertScans;
    public float alertScanRange;
    public float scanAlertPinRandomOffset;
    public float hideDuration;

    private int nextDestIndex;
    [HideInInspector] public Vector2 currentPosition;
    private Vector2 currentDirection;
    private float currentAngle;
    private float currentSpeed;
    private Vector2 destinationDirection;
    private Vector2 currentDestination;
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
    [HideInInspector] public bool isHiding;

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
    }

    private void Update()
    {
        RefreshDest();

        currentZone = ZoneHandler.GetCurrentZone(currentPosition);

        UpdateVigilance();

        if(Input.GetKeyDown(KeyCode.K))
        {
            Alert(15);
        }
    }

    private void RefreshDest()
    {
        if (IsNextInterestPoint())
        {
            currentDestination = SeaCoord.Planify(actualInterestPoint.submarineCompletionLocation.position);
        }
        else
        {
            currentDestination = SeaCoord.Planify(path.pathPosition[nextDestIndex].position);
        }
        destinationDirection = currentDestination - currentPosition;
        isNextInterestPoint = IsNextInterestPoint();
    }

    private void UpdateBehavior()
    {

    }

    private void UpdateCompletion()
    {
        if (isOnInterestPoint)
        {
            if(currentCompletionTimeSpend < actualInterestPoint.submarineCompletionTime)
            {
                currentCompletionTimeSpend += Time.fixedDeltaTime;
            }
            else
            {
                CompleteActualInterestPoint();
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateCompletion();
        UpdateMovement();
    }

    private void UpdateVigilance()
    {
        if(!isInAlert)
        {
            if(vigilance > alertVigilanceStep)
            {
                isInAlert = true;
            }
            timeRemainingBeforeNextAlertScan = 0;
        }
        else
        {
            if (vigilance <= 0)
            {
                isInAlert = false;
            }

            if(timeRemainingBeforeNextAlertScan > 0)
            {
                timeRemainingBeforeNextAlertScan -= Time.deltaTime;
            }
            else
            {
                if(StartAlertScan())
                {
                    if(currentZone.depth == Zone.Depth.Deep)
                    {
                        StartCoroutine(Hide());
                    }
                }
                timeRemainingBeforeNextAlertScan = timeBetweenAlertScans;
            }
        }
    }

    public void Alert(float vigilanceIncrease)
    {
        vigilance += vigilanceIncrease;
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

    private IEnumerator Hide()
    {
        Debug.Log("hide");
        isHiding = true;
        yield return new WaitForSeconds(hideDuration);
        isHiding = false;
    }

    private void CompleteActualInterestPoint()
    {
        actualInterestPoint.isComplete = true;
        if(actualInterestPointIndex < interestPoints.Count)
        {
            actualInterestPointIndex++;
            actualInterestPoint = interestPoints[actualInterestPointIndex];
        }

        if (nextDestIndex < path.pathPosition.Count - 1)
        {
            nextDestIndex++;
        }
    }

    private void UpdateMovement()
    {
        if(!isHiding)
        {
            if ((Vector2.Distance(currentPosition, currentDestination) < pointReachRange && !isNextInterestPoint)
                || isNextInterestPoint && Vector2.Distance(currentPosition, currentDestination) < actualInterestPoint.submarineCompletionRange)
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

    private bool IsNextInterestPoint()
    {
        bool isInterestPoint = false;
        for (int i = 0; i < interestPoints.Count; i++)
        {
            if(interestPoints[i].pathIndex == nextDestIndex)
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

}
