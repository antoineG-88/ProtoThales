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
    private int currentTurnSide;

    private float vigilance;
    private bool isUnderThermocline;

    void Start()
    {
        nextDestIndex = 1;
        currentPosition = SeaCoord.Planify(path.pathPosition[0].position);
        transform.position = SeaCoord.GetFlatCoord(currentPosition);
        currentAngle = 0;
        currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
    }

    void Update()
    {
        destinationDirection = SeaCoord.Planify(path.pathPosition[nextDestIndex].position) - currentPosition;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (Vector2.Distance(currentPosition, SeaCoord.Planify(path.pathPosition[nextDestIndex].position)) < pointReachRange)
        {
            if(nextDestIndex < path.pathPosition.Count - 1)
            {
                nextDestIndex++;
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
        }

        MoveForward(currentSpeed);
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
    }

}
