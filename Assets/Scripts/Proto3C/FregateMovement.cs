using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FregateMovement : BatimentMovement
{
    public float movementSpeed;
    public float accelerationForce;
    public float turnSpeed;
    public ParticleSystem thrustParticle;

    private int currentTurnSide;
    private float currentMaxSpeed;
    private FregateHandler fregateHandler;

    public override void Start()
    {
        base.Start();
        fregateHandler = GetComponent<FregateHandler>();
        currentSpeed = 0;
        currentMaxSpeed = movementSpeed;
    }

    public override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
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

        if (thrustParticle.isPlaying)
        {
            if (reachedDest)
            {
                thrustParticle.Stop();
            }
        }
        else
        {
            if (!reachedDest)
            {
                thrustParticle.Play();
            }
        }

        MoveForward(currentSpeed);
    }
}
