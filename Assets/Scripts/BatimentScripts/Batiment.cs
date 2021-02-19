using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batiment : MonoBehaviour
{
    public GameObject destPreview;
    public float distanceToStop;
    public float zoneDetectionDistance;

    [HideInInspector] public Vector2 currentDestination;
    [HideInInspector] public Vector2 currentPosition;
    [HideInInspector] public Vector2 currentDirection;

    protected float currentAngle;
    protected LineRenderer destinationLine;
    protected bool reachedDest;
    protected float currentSpeed;
    protected Vector2 destinationDirection;

    public virtual void Start()
    {
        destinationLine = GetComponent<LineRenderer>();
        destinationLine.enabled = false;
        currentPosition = SeaCoord.Planify(transform.position);
        currentAngle = 0;
        currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
    }

    public virtual void Update()
    {
        reachedDest = Vector2.Distance(currentPosition, currentDestination) < distanceToStop;
        destinationDirection = currentDestination - currentPosition;
        destinationDirection.Normalize();

        if(!reachedDest)
        {
            destPreview.transform.position = SeaCoord.GetFlatCoord(currentDestination) + Vector3.up * 0.01f;
            destinationLine.enabled = true;
            destinationLine.SetPosition(0, SeaCoord.GetFlatCoord(currentPosition) + Vector3.up * 0.01f);
            destinationLine.SetPosition(1, SeaCoord.GetFlatCoord(currentDestination) + Vector3.up * 0.01f);
        }
        else
        {
            destinationLine.enabled = false;
        }
    }

    /// <summary>
    /// Call at each fixedUpdate
    /// </summary>
    /// <param name="speed">The length of the movement</param>
    protected void MoveForward(float speed)
    {
        currentPosition += currentDirection * speed * Time.fixedDeltaTime;
        transform.position = SeaCoord.GetFlatCoord(currentPosition);
    }

    protected void Turn(float angle)
    {
        currentAngle += angle;
        currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
        transform.rotation = SeaCoord.SetRotation(transform.rotation, currentAngle);
    }

    public bool MoveDestination(Vector2 newDestination)
    {
        if(Vector2.Distance(currentPosition, newDestination) < distanceToStop)
        {
            return false;
        }
        else
        {
            currentDestination = newDestination;
            return true;
        }
    }
}
