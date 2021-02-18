using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : Batiment
{
    public float speed;
    public float turnSpeed;

    private int currentTurnSide;

    public float timeBetweenPoints;
    private float distance;

    private bool start;
    [HideInInspector] public bool inMovement;

    public override void Start()
    {
        base.Start();
        currentSpeed = speed;
    }

    public override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        if (!reachedDest)
        {
            inMovement = true;
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

            MoveForward(currentSpeed);

            distance = Vector2.Distance(currentPosition, currentDestination);

            if (!start)
            {
                start = true;
                timeBetweenPoints = distance / speed;
            }
        }
        else
        {
            start = false;
            inMovement = false;
        }
    }
}
