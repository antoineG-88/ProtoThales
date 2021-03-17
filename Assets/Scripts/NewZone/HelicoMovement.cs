using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicoMovement : BatimentMovement
{
    public float maxSpeed;
    public float accelerationForce;
    public float turnSpeed;

    [HideInInspector] public bool isControllable;

    private float currentMaxSpeed;
    private int currentTurnSide;

    public override void Start()
    {
        base.Start();
        currentMaxSpeed = maxSpeed;
        currentSpeed = 0;
        isControllable = true;
    }

    public override void Update()
    {
        base.Update();
    }

    public void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (isControllable)
        {
            destinationLine.enabled = true;
            destPreview.SetActive(true);
            destPreview.transform.position = SeaCoord.GetFlatCoord(currentDestination) + Vector3.up * 0.01f;
            destinationLine.enabled = true;
            destinationLine.SetPosition(0, SeaCoord.GetFlatCoord(currentPosition) + Vector3.up * 0.01f);
            destinationLine.SetPosition(1, SeaCoord.GetFlatCoord(currentDestination) + Vector3.up * 0.01f);
        }
        else
        {
            destinationLine.enabled = false;
            destPreview.SetActive(false);
        }

        if (!reachedDest)
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
        else if (currentSpeed > 0)
        {
            currentSpeed -= accelerationForce * Time.fixedDeltaTime;
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }

        MoveForward(currentSpeed);
    }
}
