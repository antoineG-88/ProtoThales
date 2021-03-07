using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterHandler : MonoBehaviour
{
    public float speed;
    public float helicopterRange;
    public float flashRange;
    public UICard helicopterCard;
    public LayerMask seaMask;
    public Transform helicopterDestPreview;

    public GameObject helicopterDisplay;
    public FregateMovement fregateMovement;
    public Transform submarine;
    public GameObject rangeDisplay;
    public GameObject winPannel;

    [HideInInspector] public bool isActive;
    private bool isChoosingHelicopterDest;
    private Vector2 currentDestination;
    private Vector2 currentPosition;
    private Vector2 currentDirection;
    private bool backToFregate;

    private void Start()
    {
        currentDestination = transform.position;

        rangeDisplay.transform.localScale = new Vector2(helicopterRange * 2, helicopterRange * 2);
        rangeDisplay.SetActive(false);
        winPannel.SetActive(false);
    }

    private void Update()
    {
        helicopterDisplay.SetActive(isActive);

        if (helicopterCard.isDragged && !helicopterCard.isHovered && !isActive)
        {
            isChoosingHelicopterDest = true;
            currentDestination = SeaCoord.Planify(InputDuo.SeaRaycast(seaMask, true).point);
            helicopterDestPreview.gameObject.SetActive(true);
            helicopterDestPreview.position = SeaCoord.GetFlatCoord(currentDestination) + Vector3.up * 0.1f;
        }

        rangeDisplay.SetActive(isChoosingHelicopterDest);

        if(helicopterCard.isDropped && isChoosingHelicopterDest && !isActive)
        {
            if (Vector2.Distance(SeaCoord.Planify(InputDuo.SeaRaycast(seaMask, true).point), fregateMovement.currentPosition) < helicopterRange)
            {
                currentDestination = SeaCoord.Planify(InputDuo.SeaRaycast(seaMask, true).point);
                currentPosition = fregateMovement.currentPosition;
                isActive = true;
            }
            isChoosingHelicopterDest = false;
        }

        MoveHelicopter();
        Move();

        rangeDisplay.transform.position = new Vector3(SeaCoord.GetFlatCoord(fregateMovement.currentPosition).x, rangeDisplay.transform.position.y, SeaCoord.GetFlatCoord(fregateMovement.currentPosition).z);
    }

    private void MoveHelicopter()
    {
        if(isActive)
        {
            if (Vector2.Distance(currentPosition, currentDestination) > 0.1f)
            {
                currentDirection = currentDestination - currentPosition;
                currentDirection.Normalize();
                currentPosition += currentDirection * speed * Time.deltaTime;
                transform.LookAt(SeaCoord.GetFlatCoord(currentDestination) + Vector3.up);
            }
            else
            {
                helicopterDestPreview.gameObject.SetActive(false);
                backToFregate = true;

                FlashHelicopter();
            }

            if (backToFregate)
            {
                currentDestination = fregateMovement.currentPosition;

                if (Vector2.Distance(currentPosition, fregateMovement.currentPosition) < 0.1f)
                {
                    backToFregate = false;
                    isActive = false;
                }
            }
        }
        else if (!isChoosingHelicopterDest)
        {
            helicopterDestPreview.gameObject.SetActive(false);
        }
    }

    private void Move()
    {
        transform.position = SeaCoord.GetFlatCoord(currentPosition) + Vector3.up;
    }

    private void FlashHelicopter()
    {
        float distanceSubmarine = Vector2.Distance(SeaCoord.Planify(submarine.position), currentPosition);

        if (distanceSubmarine < flashRange)
        {
            winPannel.SetActive(true);
        }
    }
}
