using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PatMar : OldBatiment
{
    public float maxSpeed;
    public float slowMaxSpeed;
    public float accelerationForce;
    public float patrolRange;
    public float turnSpeed;
    public float angleDetection;
    public float standByPosOffset;
    public GameObject blockedText;

    private int currentTurnSide;

    [HideInInspector] public bool canFly;
    [HideInInspector] public bool patmarIsReloading;

    public BatimentController batimentController;

    private bool isNearDest;
    private bool hasPassedDest;
    private Vector2 patrolStartDestination;
    private bool patrolFlag;
    private float currentMaxSpeed;
    private bool isInPatrol;
    private Vector2 currentDestDirection;
    private Vector2 standByPosition;
    private bool isInStandby;
    private Vector2 previousDest;
    public override void Start()
    {
        base.Start();
        currentSpeed = maxSpeed;
        currentMaxSpeed = maxSpeed;
        canFly = true;
        patmarIsReloading = false;
        blockedText.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        if (!patmarIsReloading && !reachedDest)
        {
            canFly = true;
        }
    }

    private void FixedUpdate()
    {
        /*if(IsStormInFront(currentDirection))
        {
            Vector2 inclinedDirection1 = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDirection) + angleDetection);
            if (IsStormInFront(inclinedDirection1))
            {
                currentDestDirection = SeaCoord.GetDirectionFromAngle(Vector2.SignedAngle(Vector2.right, currentDirection) - angleDetection);
            }
            else
            {
                currentDestDirection = inclinedDirection1;
            }


        }*/
        if(isInStandby)
        {
            currentDestDirection = standByPosition - currentPosition;
            currentDestDirection.Normalize();
            currentMaxSpeed = slowMaxSpeed;
            blockedText.SetActive(true);

            if (previousDest != currentDestination)
            {
                isInStandby = false;
            }
            previousDest = currentDestination;
        }
        else if (IsStormInFront(destinationDirection))
        {
            if(!isInStandby)
            {
                isInStandby = true;
                standByPosition = currentPosition - destinationDirection * standByPosOffset;
                previousDest = currentDestination;
            }
        }
        else if (Vector2.Distance(currentPosition, currentDestination) < patrolRange)
        {
            blockedText.SetActive(false);
            isInStandby = false;
            currentMaxSpeed = slowMaxSpeed;
            currentDestDirection = destinationDirection;
        }
        else
        {
            blockedText.SetActive(false);
            isInStandby = false;
            currentMaxSpeed = maxSpeed;
            currentDestDirection = destinationDirection;
        }

        if (canFly)
        {
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

            MoveForward(currentSpeed);
        }

        if (patmarIsReloading)
        {
            transform.position = SeaCoord.GetFlatCoord(currentPosition) + Vector3.up;
            currentAngle = 0;
            currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
            transform.rotation = SeaCoord.SetRotation(transform.rotation, -currentAngle + 90);
        }
    }


    private bool IsStormInFront(Vector2 direction)
    {
        Zone zone = null;
        bool isZoneInFront = false;
        for (int i = 1; i < zoneDetectionInterval + 1; i++)
        {
            zone = ZoneHandler.GetCurrentZone(currentPosition + direction * i * (zoneDetectionDistance / zoneDetectionInterval));
            if((zone != null && zone.currentWeather == Zone.Weather.Storm) || zone == null)
            {
                isZoneInFront = true;
            }
        }
        return isZoneInFront;
    }
}
