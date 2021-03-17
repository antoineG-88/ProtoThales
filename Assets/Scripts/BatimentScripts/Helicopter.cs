using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : OldBatiment
{
    public float speed;
    public float turnSpeed;
    public FregateHandler fregateHandler;
    public Fregate fregate;
    public BatimentController batimentController;

    private int currentTurnSide;

    public float timeBetweenPoints;
    private float distance;
    private Vector2 startPosition;

    private bool startFlag;
    [HideInInspector] public bool inMovement;

    public override void Start()
    {
        base.Start();
        currentSpeed = speed;
    }

    public override void Update()
    {
        base.Update();

        if (batimentController.isDragingDest)
        {
            currentPosition = fregate.currentPosition;
            currentDestination = fregate.currentPosition;
        }
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

            startPosition = fregate.currentPosition;

            if (!startFlag)
            {
                startFlag = true;
                timeBetweenPoints = distance / speed;
            }

            if (fregateHandler.helicopterCoolingDown)
            {
                currentDestination = startPosition;
            }
        }
        else
        {
            startFlag = false;
            inMovement = false;

            if (fregateHandler.helicopterCoolingDown)
            {
                currentDestination = startPosition;
            }
        }
    }
}
