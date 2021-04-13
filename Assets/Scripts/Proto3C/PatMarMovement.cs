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

    public float speedWindIncreaseRatio;
    public float sideWindIgnorance;
    public AudioSource source;
    public AudioClip flyingSound;
    public float pitchRatioForSpeed;
    public float maxSpeedPitch;

    private int currentTurnSide;

    [HideInInspector] public bool canFly;
    [HideInInspector] public bool patmarIsReloading;

    private float currentMaxSpeed;
    private Vector2 currentDestDirection;
    public float windBonusSpeedRatio;

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

        UpdateWindBonus();
    }
    private void UpdateWindBonus()
    {
        if(currentZone != null && currentZone.currentWeather == TerrainZone.Weather.Wind)
        {
            float ratio = Mathf.Cos(Mathf.Deg2Rad * (currentAngle - currentZone.windAngle));

            windBonusSpeedRatio = Mathf.Pow(ratio, sideWindIgnorance) * speedWindIncreaseRatio;
        }
        else
        {
            windBonusSpeedRatio = 0;
        }
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(currentPosition, currentDestination) < patrolRange)
        {
            currentDestDirection = destinationDirection;
            currentMaxSpeed = Mathf.Min(slowMaxSpeed, maxSpeed + (maxSpeed * windBonusSpeedRatio));
        }
        else
        {
            currentDestDirection = destinationDirection;
            if (Vector2.Angle(currentDirection, destinationDirection) < 45)
            {
                currentMaxSpeed = maxSpeed + (maxSpeed * windBonusSpeedRatio);
            }
            else
            {
                currentMaxSpeed = Mathf.Min(slowMaxSpeed, maxSpeed + (maxSpeed * windBonusSpeedRatio));
            }
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

            source.pitch = maxSpeedPitch + (currentSpeed - maxSpeed) * pitchRatioForSpeed;

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
