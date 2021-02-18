using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    public SubmarinePath path;
    public List<InterestPoint> interestPoints;
    public float pointReachRange;
    [Range(1f, 100f)] public  float maxVigilance;
    [Range(1f, 100f)] public float alertVigilanceStep;
    public float maxSpeed;
    public float acceleration;
    public float turnSpeed;

    private int nextDestIndex;
    private Vector2 currentPosition;
    private Vector2 currentDirection;
    private float currentAngle;
    private float currentSpeed;
    private Vector2 destinationDirection;
    private Vector2 currentDestination;
    private int currentTurnSide;
    private bool isOnInterestPoint;
    private bool isNextInterestPoint;
    private InterestPoint actualInterestPoint;
    private int actualInterestPointIndex;

    private float vigilance;
    [HideInInspector] public bool isUnderThermocline;
    private float currentCompletionTimeSpend;

    void Start()
    {
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
        if(IsNextInterestPoint())
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
        if((Vector2.Distance(currentPosition, currentDestination) < pointReachRange && !isNextInterestPoint)
            || IsNextInterestPoint() && Vector2.Distance(currentPosition, currentDestination) < actualInterestPoint.submarineCompletionRange)
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
                }
            }
        }
        else
        {
            if (currentSpeed <= maxSpeed)
            {
                if (currentSpeed < maxSpeed - acceleration * Time.fixedDeltaTime)
                {
                    currentSpeed += acceleration * Time.fixedDeltaTime;
                }
                else
                {
                    currentSpeed = maxSpeed;
                }
            }
            else
            {
                if (currentSpeed > maxSpeed + acceleration * Time.fixedDeltaTime)
                {
                    currentSpeed -= acceleration * Time.fixedDeltaTime;
                }
                else
                {
                    currentSpeed = maxSpeed;
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

            if(isNextInterestPoint)
            {
                currentCompletionTimeSpend = 0;
            }
            isOnInterestPoint = false;
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
