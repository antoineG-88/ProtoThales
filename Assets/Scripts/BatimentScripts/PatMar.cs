using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatMar : Batiment
{

    public float speed;
    public float turnSpeed;

    private int currentTurnSide;

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
}
