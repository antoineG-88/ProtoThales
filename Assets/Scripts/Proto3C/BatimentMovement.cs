using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatimentMovement : MonoBehaviour
{
    public GameObject destPreview;
    public float distanceToStop;
    public LayerMask seaMask;
    public UICard destinationCard;
    public bool isStandard;

    [HideInInspector] public Vector2 currentDestination;
    [HideInInspector] public Vector2 currentPosition;
    [HideInInspector] public Vector2 currentDirection;

    protected float currentAngle;
    protected LineRenderer destinationLine;
    [HideInInspector] public bool reachedDest;
    protected float currentSpeed;
    protected Vector2 destinationDirection;

    [HideInInspector] public bool canChangeDestination;
    [HideInInspector] public TerrainZone currentZone;

    public virtual void Start()
    {
        canChangeDestination = true;

        destinationLine = GetComponent<LineRenderer>();
        destinationLine.enabled = false;
        currentDestination = SeaCoord.Planify(destPreview.transform.position);

        currentPosition = SeaCoord.Planify(transform.position);
        currentAngle = 0;
        currentDirection = SeaCoord.GetDirectionFromAngle(currentAngle);
        currentDestination = SeaCoord.Planify(transform.position);
    }

    public virtual void Update()
    {
        currentZone = TerrainZoneHandler.GetCurrentZone(currentPosition, currentZone);
        UpdateHasReachedDest();
        destinationDirection = currentDestination - currentPosition;
        destinationDirection.Normalize();

        if (isStandard)
        {
            if (!reachedDest && !destinationCard.isDragged)
            {
                destPreview.transform.position = SeaCoord.GetFlatCoord(currentDestination) + Vector3.up * 0.01f;
                destinationLine.enabled = true;
                destinationLine.SetPosition(0, SeaCoord.GetFlatCoord(currentPosition) + Vector3.up * 0.01f);
                destinationLine.SetPosition(1, SeaCoord.GetFlatCoord(currentDestination) + Vector3.up * 0.01f);
            }

            if (reachedDest && !destinationCard.isDragged)
            {
                destinationLine.enabled = false;
            }

            if (destinationCard.isDropped || (InputDuo.tapHold && destinationCard.isSelected))
            {
                currentDestination = SeaCoord.Planify(InputDuo.SeaRaycast(seaMask, !GameManager.useMouseControl).point);
                GameManager.cameraController.MoveCameraWithEdge();
            }

            if (destinationCard.isFocused && InputDuo.tapUp && destinationCard.isSelected)
            {
                destinationCard.Deselect();
            }

            if (destinationCard.isDragged)
            {
                destPreview.transform.position = SeaCoord.GetFlatCoord(InputDuo.SeaRaycast(seaMask, !GameManager.useMouseControl).point);
                destinationLine.enabled = true;
                destinationLine.SetPosition(0, SeaCoord.GetFlatCoord(currentPosition) + Vector3.up * 0.01f);
                destinationLine.SetPosition(1, SeaCoord.GetFlatCoord(InputDuo.SeaRaycast(seaMask, !GameManager.useMouseControl).point) + Vector3.up * 0.01f);
            }
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
        if (Vector2.Distance(currentPosition, newDestination) < distanceToStop)
        {
            return false;
        }
        else
        {
            currentDestination = newDestination;
            return true;
        }
    }

    public void UpdateHasReachedDest()
    {
        reachedDest = Vector2.Distance(currentPosition, currentDestination) < distanceToStop;
    }
}
