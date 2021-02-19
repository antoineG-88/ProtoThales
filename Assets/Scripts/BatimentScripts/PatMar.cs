using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatMar : Batiment
{

    public float speed;
    public float turnSpeed;

    private int currentTurnSide;

    [HideInInspector] public bool canFly;
    [HideInInspector] public bool arrivedAtDestination;
    [HideInInspector] public bool patmarIsReloading;

    public BatimentController batimentController;

    public override void Start()
    {
        base.Start();
        currentSpeed = speed;
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
        if (canFly)
        {
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
        }
        if (patmarIsReloading)
        {
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            currentAngle = 0;
            currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
        }
    }
}
