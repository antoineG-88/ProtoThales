using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BatimentController : MonoBehaviour
{
    public GameObject selectionHighlighter;
    public float minDistanceToDrag;
    public LayerMask elementsLayer;
    public LayerMask surfaceLayer;
    public GameObject destinationPreview;
    public PatMarHandler patMarHandler;

    [HideInInspector] public Batiment batimentSelected;
    private Touch touch;
    private LineRenderer movementLine;

    [HideInInspector] public bool isDragingDest;
    private TouchPhase lastTouchPhase;
    private Vector2 startTouchPos;
    private Vector2 touchMovement;
    private bool isOverUI;

    void Start()
    {
        movementLine = GetComponent<LineRenderer>();
        movementLine.enabled = false;
        selectionHighlighter.SetActive(false);
        destinationPreview.SetActive(false);
    }


    void Update()
    {
        if (InputDuo.tapDown)
        {
            if (Input.GetButtonDown("LeftClick"))
            {
                startTouchPos = Input.mousePosition;
            }
            else
            {
                startTouchPos = InputDuo.touch.position;
            }
            isOverUI = EventSystem.current.IsPointerOverGameObject(/*Input.GetTouch(0).fingerId*/);
        }
        if (InputDuo.tapHold)
        {
            if(Input.GetButton("LeftClick"))
            {
                touchMovement = (Vector2)Input.mousePosition - startTouchPos;
            }
            else
            {
                lastTouchPhase = InputDuo.touch.phase;
                touchMovement = InputDuo.touch.position - startTouchPos;
            }
        }

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }

        Destination();
        Selection();
    }

    private void Selection()
    {
        if (InputDuo.tapUp && !isOverUI && touchMovement.magnitude < 10f && !patMarHandler.isWaitingForReleasePosChoice)
        {
            RaycastHit touchHit = InputDuo.SeaRaycast(elementsLayer, touch.phase == TouchPhase.Ended);
            if (touchHit.collider != null)
            {
                batimentSelected = touchHit.collider.transform.GetComponentInParent<Batiment>();
                selectionHighlighter.SetActive(true);
            }
            else
            {
                selectionHighlighter.SetActive(false);
                batimentSelected = null;
            }
        }

        if(batimentSelected != null)
        {
            selectionHighlighter.transform.position = SeaCoord.GetFlatCoord(batimentSelected.transform.position);
        }
    }

    private void Destination()
    {
        if(batimentSelected != null)
        {
            RaycastHit touchHit;
            if (InputDuo.tapDown)
            {
                touchHit = InputDuo.SeaRaycast(elementsLayer, !Input.GetButton("LeftClick"));
                if (touchHit.collider != null)
                {
                    if (batimentSelected.transform == touchHit.collider.transform.parent)
                    {
                        isDragingDest = true;
                    }
                }
            }


            if (isDragingDest)
            {
                touchHit = InputDuo.SeaRaycast(surfaceLayer, !Input.GetButton("LeftClick"));
                Vector2 pointedPosition = SeaCoord.Planify(touchHit.point);
                if (Vector2.Distance(batimentSelected.currentPosition, pointedPosition) > minDistanceToDrag)
                {
                    destinationPreview.SetActive(true);
                    destinationPreview.transform.position = SeaCoord.GetFlatCoord(pointedPosition);
                    movementLine.enabled = true;
                    Vector3[] linePos = new Vector3[2];
                    linePos[0] = SeaCoord.GetFlatCoord(batimentSelected.currentPosition);
                    linePos[1] = SeaCoord.GetFlatCoord(pointedPosition);
                    movementLine.SetPositions(linePos);
                }
            }

            if (isDragingDest && InputDuo.tapUp)
            {
                touchHit = InputDuo.SeaRaycast(surfaceLayer, touch.phase == TouchPhase.Ended);
                batimentSelected.currentDestination = SeaCoord.Planify(touchHit.point);
                isDragingDest = false;

                movementLine.enabled = false;
                destinationPreview.SetActive(false);
            }
        }
    }
}
