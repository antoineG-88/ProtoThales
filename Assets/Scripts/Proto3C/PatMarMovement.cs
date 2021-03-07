using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatMarMovement : BatimentMovement
{
    public float maxSpeed;
    public float slowMaxSpeed;
    public float accelerationForce;
    public float patrolRange;
    public float turnSpeed;

    private int currentTurnSide;

    [HideInInspector] public bool canFly;
    [HideInInspector] public bool patmarIsReloading;

    private bool isNearDest;
    private bool hasPassedDest;
    private Vector2 patrolStartDestination;
    private bool patrolFlag;
    private float currentMaxSpeed;
    private bool isInPatrol;
    private Vector2 currentDestDirection;
    private Vector2 standByPosition;
    private Vector2 previousDest;
    public override void Start()
    {
        base.Start();
        currentSpeed = maxSpeed;
        currentMaxSpeed = maxSpeed;
        canFly = true;
        patmarIsReloading = false;
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
    {if (Vector2.Distance(currentPosition, currentDestination) < patrolRange)
        {
            currentMaxSpeed = slowMaxSpeed;
            currentDestDirection = destinationDirection;
        }
        else
        {
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
}
