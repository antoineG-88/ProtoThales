using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatMar : Batiment
{
    public float maxSpeed;
    public float slowMaxSpeed;
    public float accelerationForce;
    public float patrolStartRange;
    public float turnRatio;
    public float turnRatioReferenceSpeed;
    public float patrolStartAngleOffset;

    private int currentTurnSide;

    [HideInInspector] public bool canFly;
    [HideInInspector] public bool arrivedAtDestination;
    [HideInInspector] public bool patmarIsReloading;

    public BatimentController batimentController;

    private bool isNearDest;
    private bool hasPassedDest;
    private Vector2 patrolStartDestination;
    private bool patrolFlag;
    private float currentMaxSpeed;
    private bool isInPatrol;
    private Vector2 currentDestDirection;
    public override void Start()
    {
        base.Start();
        currentSpeed = maxSpeed;
        currentMaxSpeed = maxSpeed;
    }

    public override void Update()
    {
        base.Update();

        if (batimentController.isMoving && !patmarIsReloading)
        {
            canFly = true;
        }

        arrivedAtDestination = reachedDest;
    }

    private void FixedUpdate()
    {
        if(IsStormInFront())
        {
            currentDestination = currentPosition + destinationDirection * -3;
        }

        if(Vector2.Distance(currentPosition, currentDestination) < patrolStartRange + 1)
        {
            isNearDest = true;
            if (Vector2.Distance(currentPosition, currentDestination) < distanceToStop)
            {
                hasPassedDest = true;
            }
        }
        if (patmarIsReloading)
        {
            isNearDest = false;
            hasPassedDest = false;
        }

        if(hasPassedDest && isNearDest)
        {
            currentMaxSpeed = slowMaxSpeed;
            currentDestDirection = destinationDirection;
        }
        else
        {
            currentMaxSpeed = maxSpeed;
            currentDestDirection = destinationDirection;
        }

        if (currentSpeed <= currentMaxSpeed)
        {
            if (currentSpeed < currentMaxSpeed - accelerationForce * Time.fixedDeltaTime)
            {
                currentSpeed += accelerationForce * Time.fixedDeltaTime;
            }
            else
            {
                currentSpeed = currentMaxSpeed;
            }
        }
        else
        {
            if (currentSpeed > currentMaxSpeed + accelerationForce * Time.fixedDeltaTime)
            {
                currentSpeed -= accelerationForce * Time.fixedDeltaTime;
            }
            else
            {
                currentSpeed = currentMaxSpeed;
            }
        }

        if (Vector2.Angle(currentDirection, currentDestDirection) > Time.fixedDeltaTime * turnRatio * (turnRatioReferenceSpeed / (currentSpeed / 3)))
        {
            currentTurnSide = Vector2.SignedAngle(currentDirection, currentDestDirection) > 0 ? 1 : -1;
            currentAngle = Vector2.SignedAngle(Vector2.right, currentDirection) + currentTurnSide * Time.fixedDeltaTime * turnRatio * (turnRatioReferenceSpeed / (currentSpeed / 3));
        }
        else
        {
            currentDirection = currentDestDirection;
        }


        currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
        transform.rotation = SeaCoord.SetRotation(transform.rotation, -currentAngle + 90);

        MoveForward(currentSpeed);
    }


    private bool IsStormInFront()
    {
        Zone zone = ZoneHandler.GetCurrentZone(currentPosition + destinationDirection * zoneDetectionDistance);
        return (zone != null && zone.currentWeather == Zone.Weather.Storm) || zone == null;
    }
}
